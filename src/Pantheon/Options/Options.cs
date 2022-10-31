using System.Text.Json;
using System.Text.Json.Serialization;

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
				ReferenceHandler = ReferenceHandler.IgnoreCycles
			};
	}
}
