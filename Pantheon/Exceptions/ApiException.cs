using System.Net;
using System.Runtime.Serialization;

namespace Pantheon.Exceptions
{
	public class ApiException : ArgumentException
	{
		public HttpStatusCode StatusCode { get; init; }

		public ApiException()
		{
			StatusCode = HttpStatusCode.BadRequest;
		}

		public ApiException(string? message, HttpStatusCode statusCode)
			: base(message)
		{
			StatusCode = statusCode;
		}

        public ApiException(string? message, Exception? innerException)
			: this(message, innerException, HttpStatusCode.BadRequest)
        { }

        public ApiException(string? message, string? paramName)
            : this(message, paramName, HttpStatusCode.BadRequest)
        { }

        public ApiException(string? message, string? paramName, Exception? innerException)
            : this(message, paramName, innerException, HttpStatusCode.BadRequest)
        { }

        public ApiException(string? message, Exception? innerException, HttpStatusCode statusCode)
			: base(message, innerException)
		{
			StatusCode = statusCode;
		}

		public ApiException(string? message, string? paramName, HttpStatusCode statusCode)
			: base(message, paramName)
		{
			StatusCode = statusCode;
		}

		public ApiException(string? message, string? paramName, Exception? innerException, HttpStatusCode statusCode)
			: base(message, paramName, innerException)
		{
			StatusCode = statusCode;
		}
	}
}
