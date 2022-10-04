using RabbitMQ.Client.Events;

namespace Pantheon.Abstractions.Contracts
{
	public interface IMessenger
	{
		void Initialize();
		Task HandleRpcMessage(object sender, BasicDeliverEventArgs message);
		Task HandlePubSubMessage(object sender, BasicDeliverEventArgs message);
		Task Publish<T>(T message, string topic);
		void Close();
	}
}
