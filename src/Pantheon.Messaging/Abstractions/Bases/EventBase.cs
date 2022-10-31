using System.Security.Claims;

namespace Pantheon.Messaging.Abstractions.Bases
{
	public record EventBase
	{
		public ClaimsPrincipal? User { get; init; }
	}
}
