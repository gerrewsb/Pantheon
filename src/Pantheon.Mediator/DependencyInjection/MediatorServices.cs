using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pantheon.Extensions;
using Pantheon.Mediator.Abstractions.Contracts;
using Pantheon.Mediator.PipelineBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pantheon.Mediator.DependencyInjection
{
	public static class MediatorServices
	{
		public static IServiceCollection AddAllMediatorServices(this IServiceCollection services, Type assemblyMarkerType)
		{
			services.AddMediatR(assemblyMarkerType)
				.AddPipelines()
				.AddAuthorizers(assemblyMarkerType);
			
			return services;
		}

		public static IServiceCollection AddPipelines(this IServiceCollection services)
		{
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationPipelineBehavior<,>));
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

			return services;
		}

		public static IServiceCollection AddLoggingPipeline(this IServiceCollection services)
		{
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

			return services;
		}

		public static IServiceCollection AddAuthorizationPipeline(this IServiceCollection services)
		{
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationPipelineBehavior<,>));

			return services;
		}

		public static IServiceCollection AddValidationPipeline(this IServiceCollection services)
		{
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

			return services;
		}

		public static IServiceCollection AddAuthorizers(this IServiceCollection services, Type assemblyMarkerType)
		{
			services.AddServicesAsImplementingInterface(ServiceLifetime.Scoped, typeof(IAuthorizer<>), assemblyMarkerType);
			services.AddServicesAsImplementingInterface(ServiceLifetime.Scoped, typeof(IAuthorizationHandler<>), assemblyMarkerType);

			return services;
		}
	}
}
