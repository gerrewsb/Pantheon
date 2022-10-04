using System.Collections;

namespace Pantheon.Helpers
{
    /// <summary>
    /// <para>Helper class to use an <see cref="IEnumerable{T}"/> as an <see cref="IAsyncEnumerable{T}"/> in a mocked <see cref="DbSet"/></para>
    /// <para><see href="https://learn.microsoft.com/nl-nl/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN"/></para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>, IEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public T Current => _inner.Current;

		object? IEnumerator.Current => _inner.Current;

		public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());

		public bool MoveNext() => _inner.MoveNext();

		public void Reset() => _inner.Reset();

		public void Dispose() => _inner.Dispose();
	}
}
