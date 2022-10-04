using Pantheon.Enumerations;

namespace Pantheon.Configuration
{
	public class RabbitMQConfig
	{
		public string? Hostname { get; set; }
		public string? Username { get; set; }
		public string? Password { get; set; }
		public int Port { get; set; }
		public List<string> RpcQueues { get; set; } = new();
		public List<string> PublishTopics { get; set; } = new();
		public List<string> SubscribeTopics { get; set; } = new();
	}
}