using Microsoft.EntityFrameworkCore;
using Pantheon.Abstractions.Contracts;
using Pantheon.EntityFrameworkCore.Abstractions.Bases;
using Pantheon.EntityFrameworkCore.Abstractions.Contracts;

namespace Pantheon.EntityFrameworkCore.Extensions
{
	public static class QueryableExtensions
	{
		public static IQueryable<TEntity> WhereIsValid<TEntity, TKey>(this IQueryable<TEntity> query, DateTime? validFrom = null, DateTime? validUntil = null)
			where TEntity : EntityBase<TKey>, IValidatable
			where TKey : IEquatable<TKey>
		{
			return query.Where(x => x.ValidFrom != null && x.ValidFrom <= (validFrom ?? DateTime.UtcNow)
				&& ((x.ValidUntil != null && x.ValidUntil >= (validUntil ?? DateTime.UtcNow))
					|| x.ValidUntil == null));
		}

		public static IQueryable<TEntity> WithIncludes<TEntity, TKey>(this IQueryable<TEntity> query, IIncludeSpecifications<TEntity> includeSpecifications)
			where TEntity : EntityBase<TKey>
			where TKey : IEquatable<TKey>
		{
			return includeSpecifications.Includes
				.Aggregate(query, (current, include) => current.Include(include));
		}
	}
}
