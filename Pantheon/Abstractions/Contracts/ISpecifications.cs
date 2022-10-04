using System.Linq.Expressions;

namespace Pantheon.Abstractions.Contracts
{
	public interface ISpecifications<T>
	{
        // Filter Conditions
        List<Expression<Func<T, bool>>> FilterConditions { get; }

        // Order By Ascending
        Expression<Func<T, object>>? OrderBy { get; }

        // Order By Descending
        Expression<Func<T, object>>? OrderByDescending { get; }

        // Include collection to load related data
        List<Expression<Func<T, object>>> Includes { get; }

        // GroupBy expression
        Expression<Func<T, object>>? GroupBy { get; }

        //GroupBy selector
        Expression<Func<IGrouping<object, T>, object>>? GroupBySelector { get; }
    }
}
