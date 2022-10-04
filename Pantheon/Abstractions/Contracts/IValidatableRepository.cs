using Pantheon.Abstractions.Bases;

namespace Pantheon.Abstractions.Contracts
{
	public interface IValidatableRepository<TEntity, TKey>
		where TEntity : EntityBase<TKey>, IValidatable
		where TKey : IEquatable<TKey>
	{
		Task ActivateAsync(TKey id, DateTime? validUntil = null);
		Task DeactivateAsync(TKey id, DateTime? validUntil = null);
	}
}
