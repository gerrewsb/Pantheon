namespace Pantheon.Mediator.Abstractions.Contracts
{
	public interface IAuthorizer<T>
	{
		IEnumerable<IAuthorizationRequirement> Requirements { get; }
		void BuildPolicy(T instance);
	}
}
