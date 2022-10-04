using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Pantheon.Configuration;

namespace Pantheon.Helpers
{
	public static class ConnectionStringFactory
	{
		/// <summary>
		/// Gets a <see cref="DomainConfig"/> from the configuration file
		/// </summary>
		/// <param name="configSection">Section of the <see cref="IConfiguration"/> to get the <see cref="DomainConfig"/> from</param>
		/// <param name="decryptor">Optional decryptor method to decrypt the username and password for the connectionstring</param>
		/// <returns>The fully formed connectionstring that can be used in the setup of the DbContext</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static string GetConnectionString(string configSection, Func<string, string>? decryptor = null)
		{
			string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

			IConfigurationRoot configBuilder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false)
				.AddJsonFile($"appsettings.{environment}.json", optional: false)
				.Build();

			DomainConfig domainConfig = configBuilder.GetSection(configSection).Get<DomainConfig>();

			if (domainConfig.DatabaseConfig == null)
			{
				throw new ArgumentNullException(nameof(DomainConfig.DatabaseConfig), "DataBaseConfig can not be null.");
			}

			if (decryptor != null && string.IsNullOrWhiteSpace(domainConfig.Credentials?.UserID))
			{
				throw new ArgumentNullException(nameof(domainConfig.Credentials), "UserID and Password can't be null when a decryptor is provided.");
			}

			SqlConnectionStringBuilder sqlBuilder = new()
			{
				DataSource = domainConfig.DatabaseConfig.Server,
				InitialCatalog = domainConfig.DatabaseConfig.Catalog,
				UserID = decryptor != null
					? decryptor(domainConfig.Credentials!.UserID!)
					: domainConfig.Credentials?.UserID ?? string.Empty,
				Password = decryptor != null
					? decryptor(domainConfig.Credentials!.Password!)
					: domainConfig.Credentials?.Password ?? string.Empty,
				MultipleActiveResultSets = true,
				Encrypt = false
			};

			return sqlBuilder.ConnectionString;
		}
	}
}
