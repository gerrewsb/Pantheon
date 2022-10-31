using Pantheon.Mediator.Abstractions.Contracts;

namespace Pantheon.Mediator.Abstractions.Bases
{
	public abstract class AuthorizerBase<TRequest> : IAuthorizer<TRequest>
	{
		private readonly HashSet<IAuthorizationRequirement> _requirements = new();
		public IEnumerable<IAuthorizationRequirement> Requirements => _requirements;

		protected void UseRequirement(IAuthorizationRequirement? requirement)
		{
			if (requirement == null)
			{
				return;
			}

			_requirements.Add(requirement);
		}

		public abstract void BuildPolicy(TRequest instance);
	}
}
