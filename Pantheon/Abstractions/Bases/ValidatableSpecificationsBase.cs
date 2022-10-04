using Pantheon.Abstractions.Contracts;
using System.Linq.Expressions;

namespace Pantheon.Abstractions.Bases
{
	public abstract class ValidatableSpecificationsBase<T> : SpecificationsBase<T>, IValidatableSpecifications<T>
		where T : IValidatable
	{
		public ValidatableSpecificationsBase(params Expression<Func<T, bool>>[] filterConditions) : base (filterConditions)
		{ }

		/// <summary>
		/// Add a filter to the query that checks the validitiy of the IValidatable entity
		/// </summary>
		/// <param name="validFrom"></param>
		/// <param name="validUntil"></param>
		public void AddValidOnlyFilterClause(DateTime? validFrom = null, DateTime? validUntil = null)
		{
			AddFilterCondition(x => x.ValidFrom != null && x.ValidFrom <= (validFrom ?? DateTime.UtcNow)
				&& ((x.ValidUntil != null && x.ValidUntil >= (validUntil ?? DateTime.UtcNow))
					|| x.ValidUntil == null));
		}
	}
}
