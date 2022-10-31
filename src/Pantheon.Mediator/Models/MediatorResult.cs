using Pantheon.Mediator.Abstractions.Contracts;
using System.Net;

namespace Pantheon.Mediator.Models
{
	public class MediatorResult<T> : IMediatorResult
	{
		
		public T? Result { get; set; }
		public HttpStatusCode StatusCode { get; init; }
		public bool IsSuccess { get; init; }
		public string? FailureMessage { get; init; }


		/// <summary>
		/// Create a BadRequest MediatorResponse with a failure message and statuscode
		/// </summary>
		/// <param name="failures"></param>
		/// <param name="statusCode"></param>
		public MediatorResult(string failureMessage, HttpStatusCode statusCode)
		{
			IsSuccess = false;
			StatusCode = statusCode;
			FailureMessage = failureMessage;
		}

		/// <summary>
		/// Create a success MediatorResponse
		/// </summary>
		/// <param name="result"></param>
		public MediatorResult(T result)
		{
			IsSuccess = true;
			StatusCode = HttpStatusCode.OK;
			Result = result;
		}
	}
}
