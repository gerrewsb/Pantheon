using Pantheon.Enumerations;

namespace Pantheon.Configuration
{
	public class RabbitMQConfig
	{
		public string? Hostname { get; set; }
		public string? Username { get; set; }
		public string? Password { get; set; }
		public int Port { get; set; }
	}
}