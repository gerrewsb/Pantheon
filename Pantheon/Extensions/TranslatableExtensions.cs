using Pantheon.Abstractions.Contracts;
using Pantheon.Enumerations;

namespace Pantheon.Extensions
{
	public static class TranslatableExtensions
	{
		/// <summary>
		/// Get the translation of an ITranslatable object
		/// </summary>
		/// <param name="translatable"></param>
		/// <param name="languageISO"></param>
		/// <param name="defaultValue"></param>
		/// <returns>The translation for the requested language or the default translation if there is no translation available.</returns>
		public static string GetTranslation(this ITranslatable translatable, LanguageISO languageISO, string? defaultValue = null)
		{
			string defaultTranslation = $"[{translatable.GetType().Name}_{languageISO}]";

			if (!string.IsNullOrWhiteSpace(defaultValue))
			{
				defaultTranslation = defaultValue;
			}

			return languageISO switch
			{
				LanguageISO.NL => translatable.DescNL ?? defaultTranslation,
				LanguageISO.FR => translatable.DescFR ?? defaultTranslation,
				LanguageISO.EN => translatable.DescEN ?? defaultTranslation,
				LanguageISO.DE => translatable.DescDE ?? defaultTranslation,
				_ => defaultTranslation
			};
		}
	}
}
