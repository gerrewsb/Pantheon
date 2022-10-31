using MediatR;
using Microsoft.Extensions.Logging;
using Pantheon.Exceptions;
using Pantheon.Extensions;
using Pantheon.Mediator.Abstractions.Contracts;
using System.Net;

namespace Pantheon.Mediator.PipelineBehaviors
{
	public class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
        where TResponse : IMediatorResult
	{
		private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

		public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
		{
			_logger = logger;
		}

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
            var requestName = request.GetType().Name;
            var requestGuid = Guid.NewGuid().ToString();

            var requestNameWithGuid = $"{requestName} [{requestGuid}]";

            _logger.LogInformation("[START] {requestNameWithGuid}", requestNameWithGuid);
            TResponse response = default!;

            try
            {
                response = await next();
            }
            catch (ApiException apiEx)
			{
                response = (TResponse)Activator.CreateInstance(typeof(TResponse), apiEx.AggregateExceptionMessages(), apiEx.StatusCode)!;
            }
            catch (Exception ex)
			{
                response = (TResponse)Activator.CreateInstance(typeof(TResponse), ex.AggregateExceptionMessages(), HttpStatusCode.InternalServerError)!;
            }
            finally
            {
                if (response.IsSuccess)
				{
                    _logger.LogInformation("[END] {requestNameWithGuid} - [STATUS] {statusCode}", requestNameWithGuid, response.StatusCode.ToString());
                }
				else
				{
                    _logger.LogInformation("[END] {requestNameWithGuid} - [STATUS] {statusCode} - [MESSAGE] {message}", requestNameWithGuid, response.StatusCode.ToString(), response.FailureMessage);
                }
            }

            return response;
        }
	}
}
