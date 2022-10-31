using System.Linq.Expressions;

namespace Pantheon.Helpers
{
    /// <summary>
    /// <para>Helper class to use an <see cref="IEnumerable{T}"/> as an <see cref="IAsyncEnumerable{T}"/> in a mocked <see cref="DbSet"/></para>
    /// <para><see href="https://learn.microsoft.com/nl-nl/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN"/></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}
