using System.Linq.Expressions;

namespace Pantheon.Abstractions.Contracts
{
	public interface IIncludeSpecifications<T>
	{
		List<Expression<Func<T, object>>> Includes { get; }
		void Add(Expression<Func<T, object>> includeExpression);
	}
}
