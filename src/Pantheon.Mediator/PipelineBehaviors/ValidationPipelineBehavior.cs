using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Pantheon.Mediator.Abstractions.Contracts;
using System.Diagnostics;
using System.Net;

namespace Pantheon.Mediator.PipelineBehaviors
{
	public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
		where TResponse : IMediatorResult
	{
		private readonly IEnumerable<IValidator<TRequest>> _validators;

		public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
			=> _validators = validators;

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			if (!_validators.Any())
			{
				return await next();
			}

			ValidationContext<TRequest> context = new(request);

			ValidationResult[] validationResults = await Task.WhenAll(
				_validators.Select(v => 
					v.ValidateAsync(context, cancellationToken)));

			List<string> failures = validationResults
				.Where(x => x.Errors.Any())
				.SelectMany(x => x.Errors)
				.Select(x => x.ErrorMessage)
				.ToList();

			string failureMessage = failures.Aggregate((current, next) => current += Environment.NewLine + next);

			if (failures.Any())
			{
				return (TResponse)Activator.CreateInstance(typeof(TResponse), failureMessage, HttpStatusCode.BadRequest)!;
			}

			return await next();
		}
	}
}
