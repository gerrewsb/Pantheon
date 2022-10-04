using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pantheon.Enumerations;
using System.Security.Claims;

namespace Pantheon.Extensions
{
	public static class HttpContextAccessorExtensions
	{
		/// <summary>
		/// <para>Get a headervalue from the <see cref="HttpContextAccessor"/></para>
		/// <para>The method checks if the header exists, if not the defaultvalue is returned</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="httpContextAccessor"></param>
		/// <param name="header"></param>
		/// <param name="defaultValue"></param>
		/// <returns>The value of the header or the default value</returns>
		public static T GetHeaderValue<T>(this IHttpContextAccessor httpContextAccessor, string header, T defaultValue)
		{
			if (httpContextAccessor.HttpContext?.Request.Headers.Any(x => x.Key == header) != true)
			{
				return defaultValue;
			}

			httpContextAccessor.HttpContext.Request.Headers.TryGetValue(header, out StringValues headerValue);
			return headerValue.FirstOrDefault().ConvertToValue(defaultValue); ;
		}

		/// <summary>
		/// <para>Get the value of a claim in the <see cref="HttpContextAccessor"/></para>
		/// <para>If the claim doesn't exist, a defaultvalue is returned</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="httpContextAccessor"></param>
		/// <param name="claimType"></param>
		/// <param name="defaultValue"></param>
		/// <returns>The value of the claim or the default value</returns>
		public static T GetClaimValue<T>(this IHttpContextAccessor httpContextAccessor, string claimType, T defaultValue)
		{
			if (httpContextAccessor.HttpContext?.User.Claims.Any(x => x.Type == claimType) != true)
			{
				return defaultValue;
			}

			return httpContextAccessor.HttpContext.User.Claims
				.Where(x => x.Type == claimType)
				.Select(x => x.Value)
				.FirstOrDefault()
				.ConvertToValue(defaultValue);
		}

		/// <summary>
		/// Gets the LanguageISO of the current member from the claim or header
		/// </summary>
		/// <param name="httpContextAccessor"></param>
		/// <returns><see cref="LanguageISO"/></returns>
		public static LanguageISO GetMemberLanguageISO(this IHttpContextAccessor httpContextAccessor)
			=> httpContextAccessor.HttpContext?.User?.Claims?.Any(x => x.Type == ClaimTypes.Locality) == true
				? httpContextAccessor.GetClaimValue(ClaimTypes.Locality, LanguageISO.EN)
				: httpContextAccessor.GetHeaderValue("Accept-Language", LanguageISO.EN);

		/// <summary>
		/// Gets the MemberID of the current member from the claim or header
		/// </summary>
		/// <param name="httpContextAccessor"></param>
		/// <returns></returns>
		public static int GetCurrentMemberID(this IHttpContextAccessor httpContextAccessor)
			=> httpContextAccessor.HttpContext?.User?.Claims?.Any(x => x.Type == "memberID") == true
				? httpContextAccessor.GetClaimValue("memberID", default(int))
				: httpContextAccessor.GetHeaderValue("X-MemberID", default(int));
	}
}
