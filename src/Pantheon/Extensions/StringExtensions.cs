using System.ComponentModel;
using System.Globalization;

namespace Pantheon.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Extension to convert a string value to a typed value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="defaultValue"></param>
		public static T ConvertToValue<T>(this string? value, T defaultValue)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return defaultValue;
			}

			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

			try
			{
				if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?)
					|| typeof(T) == typeof(double) || typeof(T) == typeof(double)
					|| typeof(T) == typeof(float) || typeof(T) == typeof(float?))
				{
					value = value.Replace(',', '.');
				}

				var convertedValue = converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);

				return convertedValue == null
					? defaultValue
					: (T)convertedValue;
			}
			catch
			{
				return defaultValue;
			}
		}
	}
}
