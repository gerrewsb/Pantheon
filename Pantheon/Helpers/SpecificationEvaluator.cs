using Microsoft.EntityFrameworkCore;
using Pantheon.Abstractions.Contracts;
using Pantheon.Abstractions.Bases;

namespace Pantheon.Helpers
{
    /// <summary>
    /// Evaluator of the <see cref="ISpecifications{TEntity}"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
	public sealed class SpecificationEvaluator<TEntity, TKey>
        where TEntity : EntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Get the EF Core query based on the <see cref="TEntity"/> and the optional <see cref="ISpecifications{TEntity}"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="specifications"></param>
        /// <returns>The <see cref="IQueryable{TEntity}"/> with all the specification applied</returns>
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> query, ISpecifications<TEntity>? specifications = null)
        {
            // Do not apply anything if specifications is null
            if (specifications == null)
            {
                return query;
            }

            query = specifications.FilterConditions
                .Aggregate(query, (current, filter) => current.Where(filter));

            // Includes
            query = specifications.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering
            if (specifications.OrderBy != null)
            {
                query = query.OrderBy(specifications.OrderBy);
            }
            else if (specifications.OrderByDescending != null)
            {
                query = query.OrderByDescending(specifications.OrderByDescending);
            }

            return query;
        }

        /// <summary>
        /// Get the EF Core query based on the <see cref="TEntity"/> and the required <see cref="ISpecifications{TEntity}"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="specifications"></param>
        /// <returns>The <see cref="IQueryable{object}"/> with the specifications and grouping applied</returns>
        public static IQueryable<object> GetQueryWithGrouping(IQueryable<TEntity> query, ISpecifications<TEntity> specifications)
		{
            // Apply GroupBy
            if (specifications.GroupBy != null && specifications.GroupBySelector != null)
            {
                return GetQuery(query, specifications)
                    .GroupBy(specifications.GroupBy)
                    .Select(specifications.GroupBySelector);
            }

            return GetQuery(query, specifications);
        }
    }
}
