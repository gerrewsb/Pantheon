using System.Text;

namespace Pantheon.Extensions
{
	public static class ExceptionExtensions
	{
        /// <summary>
        /// Aggregates the messages.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static string AggregateExceptionMessages(this Exception ex)
            => ex.GetInnerExceptions()
                .Aggregate(new StringBuilder(), (sb, e)
                    => sb.AppendLine(e.Message), sb
                        => sb.ToString());


        /// <summary>
        /// Gets the inner exceptions.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <returns></returns>
        public static IEnumerable<Exception> GetInnerExceptions(this Exception? ex, int maxDepth = 5)
        {
            if (ex == null || maxDepth <= 0)
            {
                yield break;
            }

            yield return ex;

            if (ex is AggregateException ax)
            {
                foreach (var i in ax.InnerExceptions.SelectMany(ie => GetInnerExceptions(ie, maxDepth - 1)))
				{
                    yield return i;
                }
            }

            foreach (var i in GetInnerExceptions(ex.InnerException, maxDepth - 1))
			{
                yield return i;
            }
        }
    }
}
