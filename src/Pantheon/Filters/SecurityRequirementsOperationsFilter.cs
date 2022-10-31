using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;

namespace Pantheon.Filters
{
	public class SecurityRequirementsOperationsFilter : IOperationFilter
	{
		/// <summary>
		/// If an endpoint (or the entire controller) has the [Authorize] attribute, this filter will append 401 and 403 response types in the swagger UI
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="context"></param>
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			//IEnumerable<string?> requiredScopes = context.MethodInfo
			//	.GetCustomAttributes(true)
			//	.OfType<AuthorizeAttribute>()
			//	.Select(attr => attr.Policy)
			//	.Distinct();

			bool hasAuthorizeAttribute = context.MethodInfo
				.GetCustomAttributes(true)
				.Any(x => x.GetType() == typeof(AuthorizeAttribute));

			bool controllerHasAuthorizeAttribute = context.ApiDescription
				.ActionDescriptor
				.EndpointMetadata
				.Any(x => x.GetType() == typeof(AuthorizeAttribute));

			bool hasAllowAnonymousAttribute = context.MethodInfo
				.GetCustomAttributes(true)
				.Any(x => x.GetType() == typeof(AllowAnonymousAttribute));

			//if (requiredScopes.Any() || (controllerHasAuthorizeAttribute && !hasAllowAnonymousAttribute))
			if (hasAuthorizeAttribute || (controllerHasAuthorizeAttribute && !hasAllowAnonymousAttribute))
			{
				operation.Responses.Add("401", new() { Description = HttpStatusCode.Unauthorized.ToString() });
				operation.Responses.Add("403", new() { Description = HttpStatusCode.Forbidden.ToString() });

				//OpenApiSecurityScheme oAuthScheme = new()
				//{
				//	Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" }
				//};

				//operation.Security = new List<OpenApiSecurityRequirement>
				//{
				//	new()
				//	{
				//		[oAuthScheme] = requiredScopes.ToList()
				//	}
				//};
			}
		}
	}
}
