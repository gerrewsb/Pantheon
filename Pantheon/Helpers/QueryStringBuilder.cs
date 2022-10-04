using System.Reflection;
using System.Text;
using System.Web;

namespace Pantheon.Helpers
{
	public sealed class QueryStringBuilder
	{
		private readonly StringBuilder urlExtension = new("?");

		/// <summary>
		/// <para>Provide a model to the QueryString.</para>
		/// <para>Every valid property (not null/empty) will be added to the QueryString.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="model"></param>
		/// <returns>The current QueryStringBuilder object</returns>
		public QueryStringBuilder Add<T>(T model)
		{
			foreach (PropertyInfo prop in typeof(T).GetProperties())
			{
				object? value = prop.GetValue(model);

				if (value != null && value?.ToString() != string.Empty)
				{
					Add(prop.Name, prop.PropertyType, value!);
				}
			}

			return this;
		}

		/// <summary>
		/// <para>Provide a dictionary to the QueryString.</para>
		/// <para>Every valid key/value (not null/empty) will be added to the QueryString.</para>
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns>The current QueryStringBuilder object</returns>
		public QueryStringBuilder Add(Dictionary<string, object?> parameters)
		{
			foreach (KeyValuePair<string, object?> paramPair in parameters)
			{
				Add(paramPair.Key, paramPair.Value?.GetType(), paramPair.Value);
			}

			return this;
		}

		/// <summary>
		/// <para>Provide a single key and value to the QueryString.</para>
		/// <para>a valid key/value pair (not null/empty) will be added to the QueryString.</para>
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns>The current QueryStringBuilder object</returns>
		public QueryStringBuilder Add(string key, object? value)
		{
			Add(key, value?.GetType(), value);
			return this;
		}

		/// <summary>
		/// Return the fully compiled QueryString, this method has to be called to get the full QueryString.
		/// </summary>
		/// <returns>The querystring to be used in a HttpRequest</returns>
		public string GetUrlExtension() => urlExtension.ToString()[0..^1];

		private QueryStringBuilder Add(string key, Type? type, object? value)
		{
			if (value != null && type != null && (type == typeof(DateTime) || type == typeof(DateTime?)))
			{
				value = ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ssZ");
			}
			else if (!string.IsNullOrWhiteSpace(value?.ToString()) && type != null && (type == typeof(string)))
			{
				value = HttpUtility.UrlEncodeUnicode(value.ToString());
			}

			if (!string.IsNullOrWhiteSpace(value?.ToString()) && !string.IsNullOrWhiteSpace(key))
			{
				urlExtension.Append(key).Append('=').Append(value).Append('&');
			}

			return this;
		}		
	}
}
