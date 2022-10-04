using System.Text.Json;

namespace Pantheon.Options
{
	public static class Options
	{
		private static JsonSerializerOptions? _basicJsonSerializerOptions;

		/// <summary>
		/// Basic JsonSerializerOptions to be used in the JsonOptions setup
		/// </summary>
		public static JsonSerializerOptions BasicJsonSerializerOptions
			=> _basicJsonSerializerOptions ??=
			new()
			{
				WriteIndented = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				PropertyNameCaseInsensitive = true,
			};
	}
}
