using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Pantheon.Helpers
{
    /// <summary>
    /// <para>Helper class to use an <see cref="IEnumerable{T}"/> as an <see cref="IAsyncEnumerable{T}"/> in a mocked <see cref="DbSet"/></para>
    /// <para><see href="https://learn.microsoft.com/nl-nl/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN"/></para>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
	public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object? Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(this, new[] { expression });

            var result = typeof(Task).GetMethod(nameof(Task.FromResult))
                ?.MakeGenericMethod(expectedResultType)
                ?.Invoke(null, new[] { executionResult });

            return result != null
                ? (TResult)result
                : default!;
        }
    }
}
