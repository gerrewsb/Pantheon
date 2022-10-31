using System.Collections;
using System.Linq.Expressions;

namespace Pantheon.Helpers
{
	/// <summary>
	/// Used in tests to represent a single value as an <see cref="IQueryable"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SingleValueQueryable<T> : IQueryable<T>, IEnumerator<T>
	{
		private bool _hasExecutedOnce = false;

		public SingleValueQueryable(T value)
		{
			Current = value;
		}

		public T Current { get; set; }

		public Type ElementType => typeof(T);

		public Expression Expression => throw new NotImplementedException();

		public IQueryProvider Provider => throw new NotImplementedException();

		object IEnumerator.Current => Current!;

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public IEnumerator<T> GetEnumerator() => this;

		public bool MoveNext()
		{
			if (_hasExecutedOnce)
			{
				return false;
			}

			_hasExecutedOnce = true;
			return true;
		}

		public void Reset()
		{
			_hasExecutedOnce = false;
		}

		IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
	}
}
