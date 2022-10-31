using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Moq;
using Pantheon.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Pantheon.Test.FilterTests
{
	public class SecurityRequirementsOperationsFilterTests
	{
		private readonly SecurityRequirementsOperationsFilter _filter;

		public SecurityRequirementsOperationsFilterTests()
		{
			_filter = new();
		}

		[Fact]
		public void Context_With_AuthorizeAttribute_Should_Add_401_And_403_Responses()
		{
			OpenApiOperation operation = new();
			var schemaGenerator = new Mock<ISchemaGenerator>();
			var methodInfo = new Mock<MethodInfo>();
			methodInfo.Setup(x => x.GetCustomAttributes(true)).Returns(new[] { new AuthorizeAttribute() });
			ActionDescriptor actionDescriptor = new() { EndpointMetadata = new List<object> { new AuthorizeAttribute() } };
			ApiDescription apiDescription = new() { ActionDescriptor = actionDescriptor };
			OperationFilterContext context = new(apiDescription, schemaGenerator.Object, new(), methodInfo.Object);
			_filter.Apply(operation, context);

			operation.Responses.Should().HaveCount(2);
			operation.Responses.Should().Contain(new KeyValuePair<string, OpenApiResponse>("401", operation.Responses["401"]));
			operation.Responses.Should().Contain(new KeyValuePair<string, OpenApiResponse>("403", operation.Responses["403"]));
		}

		[Fact]
		public void Context_Without_AuthorizeAttribute_Should_Not_Add_401_And_403_Responses()
		{
			OpenApiOperation operation = new();
			var schemaGenerator = new Mock<ISchemaGenerator>();
			var methodInfo = new Mock<MethodInfo>();
			methodInfo.Setup(x => x.GetCustomAttributes(true)).Returns(Array.Empty<object>());
			ActionDescriptor actionDescriptor = new() { EndpointMetadata = new List<object> { } };
			ApiDescription apiDescription = new() { ActionDescriptor = actionDescriptor };
			OperationFilterContext context = new(apiDescription, schemaGenerator.Object, new(), methodInfo.Object);
			_filter.Apply(operation, context);

			operation.Responses.Should().HaveCount(0);
		}
	}
}
