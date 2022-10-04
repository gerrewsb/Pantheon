namespace Pantheon.Abstractions.Contracts
{
	public interface IValidatableSpecifications<T> where T : IValidatable
	{
		void AddValidOnlyFilterClause(DateTime? validFrom, DateTime? validUntil);
	}
}
