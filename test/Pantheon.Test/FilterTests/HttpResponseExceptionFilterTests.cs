using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Pantheon.Exceptions;
using Pantheon.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Pantheon.Test.FilterTests
{
	public class HttpResponseExceptionFilterTests
	{
		private readonly HttpResponseExceptionFilter _filter;
		private readonly ActionContext _actionContext;

		public HttpResponseExceptionFilterTests()
		{
			ILogger<HttpResponseExceptionFilter> logger = new LoggerFactory().CreateLogger<HttpResponseExceptionFilter>();

			_filter = new(logger);

			var httpContext = new Mock<HttpContext>();
			var routeData = new Mock<RouteData>();
			var actionDescriptor = new Mock<ActionDescriptor>();
			_actionContext = new ActionContext(httpContext.Object, routeData.Object, actionDescriptor.Object);
		}

		[Fact]
		public void ApiException_Should_Result_In_ObjectResult()
		{
			ApiException exception = new("An ApiException occurred.", "ApiException", HttpStatusCode.BadRequest);

			ExceptionContext context = new(_actionContext, new List<IFilterMetadata>())
			{
				Exception = exception
			};

			_filter.OnException(context);

			context.Result.Should().BeOfType<ObjectResult>();
			(context.Result as ObjectResult)!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
		}

		[Fact]
		public void InvalidOperationException_Should_Result_In_ObjectResult()
		{
			InvalidOperationException exception = new("An InvalidOperationException occurred.");

			ExceptionContext context = new(_actionContext, new List<IFilterMetadata>())
			{
				Exception = exception
			};

			_filter.OnException(context);

			context.Result.Should().BeOfType<ObjectResult>();
			(context.Result as ObjectResult)!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
		}

		[Fact]
		public void ValidationException_Should_Result_In_ObjectResult()
		{
			ValidationException exception = new("A ValidationException occurred.");

			ExceptionContext context = new(_actionContext, new List<IFilterMetadata>())
			{
				Exception = exception
			};

			_filter.OnException(context);

			context.Result.Should().BeOfType<ObjectResult>();
			(context.Result as ObjectResult)!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
		}

		[Fact]
		public void Exception_Should_Result_In_ObjectResult_With_StatusCode_InternalServerError()
		{
			Exception exception = new("A generic Exception occurred.");

			ExceptionContext context = new(_actionContext, new List<IFilterMetadata>())
			{
				Exception = exception
			};

			_filter.OnException(context);

			context.Result.Should().BeOfType<ObjectResult>();
			(context.Result as ObjectResult)!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
		}
	}
}
