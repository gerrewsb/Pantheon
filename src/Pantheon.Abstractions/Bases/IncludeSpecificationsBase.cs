using Pantheon.Abstractions.Contracts;
using System.Linq.Expressions;

namespace Pantheon.Abstractions.Bases
{
	public abstract class IncludeSpecificationsBase<T> : IIncludeSpecifications<T>
	{
		private readonly List<Expression<Func<T, object>>> _includeCollection = new();
		public List<Expression<Func<T, object>>> Includes => _includeCollection;

		public void Add(Expression<Func<T, object>> includeExpression)
			=> Includes.Add(includeExpression);
	}
}
