using Pantheon.Enumerations;
using System.Security.Claims;

namespace Pantheon.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		public static LanguageISO GetUserLanguage(this ClaimsPrincipal? user, LanguageISO defaultLanguage = LanguageISO.EN)
		{
			if (user?.Claims?.Any() != true)
			{
				return defaultLanguage;
			}

			if (!user.HasClaim(x => x.Type == "Language"))
			{
				return defaultLanguage;
			}

			int claimValue = Convert.ToInt32(user.Claims.FirstOrDefault(x => x.Type == "Language")!.Value);
			return (LanguageISO)claimValue;
		}
	}
}
