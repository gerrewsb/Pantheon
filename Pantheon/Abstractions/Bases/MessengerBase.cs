using Pantheon.Abstractions.Contracts;
using Pantheon.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;
using System.Text.Json;

namespace Pantheon.Abstractions.Bases
{
	/// <summary>
	/// Baseclass to work with RabbitMQ messaging
	/// </summary>
	public abstract class MessengerBase : IMessenger
	{
		private IConnection? _connection;
		private readonly List<(string Topic, IModel Channel)> _publishChannels = new();
		private readonly Func<string, string>? _decryptor;
		private readonly RabbitMQConfig _configuration;

		protected List<(string QueueName, IModel Channel, IBasicProperties Properties)> RpcChannels { get; } = new();
		protected List<(string Topic, IModel Channel)> SubscribeChannels = new();
		protected ILogger Logger;
		protected readonly JsonSerializerOptions? JsonSerializerOptions;

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="configuration"><see cref="RabbitMQConfig"/> from the application's configuration</param>
		/// <param name="logger"><see cref="Serilog"/> logger</param>
		/// <param name="decryptor">An optional decryptor to decrypt username/password if these parameters are encrypted in the application's configuration</param>
		/// <param name="jsonSerializerOptions">Optional <see cref="System.Text.Json.JsonSerializerOptions"/> to serialize and deserialize the messages</param>
		public MessengerBase(RabbitMQConfig configuration, ILogger logger, Func<string, string>? decryptor = null, JsonSerializerOptions? jsonSerializerOptions = null)
		{
			_configuration = configuration;
			_decryptor = decryptor;
			Logger = logger;
			JsonSerializerOptions = jsonSerializerOptions;
			Initialize();
		}

		/// <summary>
		/// <para>Initilization of the Messenger client.</para>
		/// All RPC Queues, Publish topics and Subscribe topics are taken from the <see cref="RabbitMQConfig"/> and a handler is created for each one.
		/// </summary>
		public void Initialize()
		{
			_connection = GetConnectionFactory().CreateConnection();

			foreach (string rpcQueue in _configuration.RpcQueues)
			{
				var correlationID = Guid.NewGuid().ToString();
				var channel = _connection.CreateModel();
				var queueName = channel.QueueDeclare().QueueName;
				var consumer = new AsyncEventingBasicConsumer(channel);
				var props = channel.CreateBasicProperties();
				props.CorrelationId = correlationID;
				props.ReplyTo = queueName;
				props.Type = rpcQueue;
				channel.BasicConsume(consumer, queueName, true);
				consumer.Received += HandleRpcMessage;
				RpcChannels.Add((rpcQueue, channel, props));
			}

			foreach (var topic in _configuration.PublishTopics)
			{
				var channel = _connection.CreateModel();
				channel.ExchangeDeclare(topic.ToString(), ExchangeType.Fanout);
				_publishChannels.Add((topic, channel));
			}

			foreach (var topic in _configuration.SubscribeTopics)
			{
				var channel = _connection.CreateModel();
				channel.ExchangeDeclare(topic.ToString(), ExchangeType.Fanout);
				var queueName = channel.QueueDeclare().QueueName;
				channel.QueueBind(queueName, topic.ToString(), string.Empty);
				var consumer = new AsyncEventingBasicConsumer(channel);
				consumer.Received += HandlePubSubMessage;
				channel.BasicConsume(queueName, true, consumer);
				SubscribeChannels.Add((topic, channel));
			}
		}

		public abstract Task HandleRpcMessage(object sender, BasicDeliverEventArgs message);

		public abstract Task HandlePubSubMessage(object sender, BasicDeliverEventArgs message);

		/// <summary>
		/// <para>Generic method to publish a message for a certain topic</para>
		/// <para>The corresponding handler for the topic is searched in the publish dictionary.</para>
		/// <para>The message won't be published if the corresponding handler for the topic is not found</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		/// <param name="topic"></param>
		/// <returns></returns>
		public Task Publish<T>(T message, string topic)
		{
			var publishChannel = GetPublishChannel(topic);

			if (publishChannel == default)
			{
				return Task.CompletedTask;
			}

			var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, JsonSerializerOptions));
			publishChannel.Channel.BasicPublish(publishChannel.Topic, string.Empty, null, body);

			return Task.CompletedTask;
		}

		private (string Topic, IModel Channel) GetPublishChannel(string topic)
			=> _publishChannels.FirstOrDefault(x => x.Topic == topic);

		/// <summary>
		/// Close the connection of the Messenger
		/// </summary>
		public void Close()
		{
			_connection?.Close();
		}

		private ConnectionFactory GetConnectionFactory()
		{
			ConnectionFactory connectionFactory = new()
			{
				HostName = _configuration.Hostname,
				DispatchConsumersAsync = true
			};

			var userName = _configuration.Username;
			var password = _configuration.Password;
			var port = _configuration.Port;

			if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
			{
				connectionFactory.UserName = _decryptor != null ? _decryptor(userName) : userName;
				connectionFactory.Password = _decryptor != null ? _decryptor(password) : password;
			}

			if (port > 0)
			{
				connectionFactory.Port = port;
			}

			return connectionFactory;
		}
	}
}
