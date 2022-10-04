using System.Text;

namespace Pantheon.Extensions
{
	public static class ExceptionExtensions
	{
        /// <summary>
        /// Get all messages from all the (inner)exceptions
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>string with all the messages</returns>
        public static string? Flatten(this Exception? ex)
        {
            if (ex == null)
			{
                return null;
			}

            StringBuilder sb = new();
            sb.AppendLine(ex.Message)
                .AppendLine();

            Exception? innerException = ex.InnerException;

            while (innerException != null)
            {
                sb.AppendLine(innerException.Message)
                    .AppendLine();
                innerException = innerException.InnerException;
            }

            return sb.ToString();
        }
    }
}
