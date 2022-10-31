using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pantheon.Mediator.Abstractions.Contracts;
using Pantheon.Mediator.Models;
using System.Collections.Concurrent;
using System.Net;
using System.Reflection;

namespace Pantheon.Mediator.PipelineBehaviors
{
	public class AuthorizationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IAuthorizedRequest<TResponse>
		where TResponse : IMediatorResult
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IEnumerable<IAuthorizer<TRequest>> _authorizers;
		private static readonly ConcurrentDictionary<Type, Type> s_requirementHandlers = new();
		private static readonly ConcurrentDictionary<Type, MethodInfo> s_handlerMethodInfo = new();

		public AuthorizationPipelineBehavior(IEnumerable<IAuthorizer<TRequest>> authorizers, IServiceProvider serviceProvider)
		{
			_authorizers = authorizers;
			_serviceProvider = serviceProvider;
		}

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			if (!_authorizers.Any())
			{
				return await next();
			}

			HashSet<IAuthorizationRequirement> requirements = new();

			foreach (IAuthorizer<TRequest> authorizer in _authorizers)
			{
				authorizer.BuildPolicy(request);

				foreach(IAuthorizationRequirement requirement in authorizer.Requirements)
				{
					requirements.Add(requirement);
				}
			}

			AuthorizationResult[] authorizationResults = await Task.WhenAll(
				requirements.Select(x =>
					ExecuteAuthorizationHandler(x, cancellationToken)));

			List<string> failures = authorizationResults
				.Where(x => !x.IsAuthorized)
				.Select(x => x.FailureMessage!)
				.ToList();

			if (failures.Any())
			{
				return (TResponse)Activator.CreateInstance(typeof(TResponse), failures, HttpStatusCode.Unauthorized)!;
			}

			return await next();
		}

		private Task<AuthorizationResult> ExecuteAuthorizationHandler(IAuthorizationRequirement requirement, CancellationToken cancellationToken)
		{
			Type requirementType = requirement.GetType();
			Type handlerType = FindHandlerType(requirement);

			IEnumerable<object>? handlers = _serviceProvider
				.GetRequiredService(typeof(IEnumerable<>)
				.MakeGenericType(handlerType)) as IEnumerable<object>;

			if (handlers?.Any() != true)
			{
				return Task.FromResult(AuthorizationResult.Fail($"Could not find an authorization handler implementation for requirement type '{requirementType.Name}'"));
			}

			if (handlers.Count() > 1)
			{
				return Task.FromResult(AuthorizationResult.Fail($"Multiple authorization handler implementations were found for requirement type '{requirementType.Name}'"));
			}

			object serviceHandler = handlers.First();
			Type serviceHandlerType = serviceHandler.GetType();

			var methodInfo = s_handlerMethodInfo.GetOrAdd(serviceHandlerType,
				handlerMethodKey =>
				{
					return serviceHandlerType.GetMethods()
						.Where(x => x.Name == nameof(IAuthorizationHandler<IAuthorizationRequirement>.Handle))
						.FirstOrDefault()!;
				});

			return (Task<AuthorizationResult>)methodInfo.Invoke(serviceHandler, new object [] { requirement, cancellationToken })!;
		}

		private Type FindHandlerType(IAuthorizationRequirement requirement)
		{
			Type requirementType = requirement.GetType();

			Type handlerType = s_requirementHandlers.GetOrAdd(requirementType,
				requirementTypeKey =>
				{
					Type wrapperType = typeof(IAuthorizationHandler<>).MakeGenericType(requirementTypeKey);
					return wrapperType;
				});

			return handlerType;
		}
	}
}
