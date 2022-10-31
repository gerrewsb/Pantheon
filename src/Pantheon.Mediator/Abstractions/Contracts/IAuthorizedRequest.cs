using MediatR;
using System.Security.Claims;

namespace Pantheon.Mediator.Abstractions.Contracts
{
	public interface IAuthorizedRequest<TResponse> : IRequest<TResponse>
	{ 
		ClaimsPrincipal User { get; }
	}
}
