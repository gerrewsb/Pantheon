using Pantheon.Abstractions.Contracts;
using System.Linq.Expressions;

namespace Pantheon.Abstractions.Bases
{
    /// <summary>
    /// <para>Generic Specifications , can be easily used for filter expressions</para>
    /// <para>For additional expressions, class needs to be derived.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpecificationsBase<T> : ISpecifications<T>
    {
        private readonly List<Expression<Func<T, object>>> _includeCollection = new();
        private readonly List<Expression<Func<T, bool>>> _filterConditions = new();

        public SpecificationsBase(params Expression<Func<T, bool>>[] filterConditions)
        {
            if (filterConditions?.Any() == true)
			{
                FilterConditions.AddRange(filterConditions);
            }
        }

        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        public Expression<Func<T, object>>? GroupBy { get; private set; }
        public Expression<Func<IGrouping<object, T>, object>>? GroupBySelector { get; private set; }
        public List<Expression<Func<T, object>>> Includes => _includeCollection;
        public List<Expression<Func<T, bool>>> FilterConditions => _filterConditions;

        /// <summary>
        /// Add an Include for the query
        /// </summary>
        /// <param name="includeExpression"></param>
        public void AddInclude(Expression<Func<T, object>> includeExpression)
            => Includes.Add(includeExpression);

        /// <summary>
        /// Apply an OrderBy for the query
        /// </summary>
        /// <param name="orderByExpression"></param>
        public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
            => OrderBy = orderByExpression;


        /// <summary>
        /// Apply an OrderByDescending for the query
        /// </summary>
        /// <param name="orderByDescendingExpression"></param>
        public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
            => OrderByDescending = orderByDescendingExpression;

        /// <summary>
        /// Add a filtercondition for the query
        /// </summary>
        /// <param name="filterExpression"></param>
        public void AddFilterCondition(Expression<Func<T, bool>> filterExpression)
            => FilterConditions.Add(filterExpression);
    }
}
