using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pantheon.Extensions
{
	public class ApiClientExtensions
	{
		/// <summary>
		/// Check the base url of <see cref="HttpClient"/><br></br>
		/// First it removes the trailing '/' character if there is any<br></br>
		/// Secondly it removes the last 3 characters of the url if the url ends with 'api'<br></br>
		/// Thridly it appends the '/' character again so the base url is a valid <see cref="Uri"/><br></br>
		/// Then it checks if it should add the 'api' suffix, if it does, it adds 'api/' at the end of the url.
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="addApiSuffix"></param>
		/// <returns>The base url for the <see cref="HttpClient"/></returns>
		public static string? CheckBaseUrl(string? baseUrl, bool addApiSuffix = false)
		{
			if (string.IsNullOrWhiteSpace(baseUrl))
			{
				return null;
			}

			baseUrl = baseUrl.ToLower();
			baseUrl = baseUrl.TrimEnd('/');

			if (baseUrl.EndsWith("api"))
			{
				//Get the substring of baseUrl without the last 3 characters => api
				baseUrl = baseUrl[..^3];
			}

			if (!baseUrl.EndsWith('/'))
			{
				baseUrl += '/';
			}

			if (addApiSuffix)
			{
				baseUrl += "api/";
			}

			return baseUrl;
		}
	}
}
