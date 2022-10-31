using Pantheon.Helpers;

namespace Pantheon.Extensions
{
	public static class ObjectExtensions
	{
		/// <summary>
		/// <para>Use this extension to convert a single class or value to an IQueryable.</para>
		/// <para>Should be used in tests only.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static IQueryable<T> ToQueryable<T>(this T instance)
		{
			return new SingleValueQueryable<T>(instance);
		}
	}
}
