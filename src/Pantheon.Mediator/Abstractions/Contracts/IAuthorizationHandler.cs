using Pantheon.Mediator.Models;

namespace Pantheon.Mediator.Abstractions.Contracts
{
	public interface IAuthorizationHandler<TRequirement>
		where TRequirement : IAuthorizationRequirement
	{
		Task<AuthorizationResult> Handle(TRequirement requirement, CancellationToken cancellationToken = default);
	}
}
