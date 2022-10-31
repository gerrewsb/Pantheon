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
		public static string GetDescription(this ITranslatable translatable, LanguageISO languageISO, string? defaultValue = null)
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

		public static void SetDefaultDescriptions(this ITranslatable translatable, string descriptionDefault)
		{
			List<string> descriptions = translatable.GetType()
				.GetProperties()
				.Where(x => x.Name.StartsWith("Desc"))
				.Select(x => x.Name)
				.ToList();

			foreach (var description in descriptions)
			{
				string? value = translatable.GetType()
					.GetProperty(description)
					?.GetValue(translatable)
					?.ToString();

				if (string.IsNullOrWhiteSpace(value))
				{
					translatable.GetType()
						.GetProperty(description)
						?.SetValue(translatable, $"[{description.Replace("Desc", "")}_{descriptionDefault}]");
				}
			}
		}
	}
}
