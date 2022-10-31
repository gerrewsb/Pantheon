using System.Net;

namespace Pantheon.Mediator.Abstractions.Contracts
{
	public interface IMediatorResult
	{
		public HttpStatusCode StatusCode { get; }
		bool IsSuccess { get; }
		public string? FailureMessage { get; }
	}
}
