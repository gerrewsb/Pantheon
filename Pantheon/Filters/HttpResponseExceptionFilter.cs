using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Pantheon.Exceptions;
using Pantheon.Extensions;
using Serilog;
using System.Net;

namespace Pantheon.Filters
{
	public class HttpResponseExceptionFilter : IExceptionFilter, IOrderedFilter
	{
		private readonly ILogger _logger;

		public int Order => int.MaxValue - 10;

		public HttpResponseExceptionFilter(ILogger logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// <para>When this filter is applied and an exception has occurred, the OnException method is called.</para>
		/// <para>All exceptions that are not of type <see cref="ApiException"/> will be converted.</para>
		/// <para>The appropriate <see cref="HttpStatusCode"/> will be applied and the ObjectResult will be set in the context.</para>
		/// </summary>
		/// <param name="context"></param>
		public void OnException(ExceptionContext context)
		{
			if (context.Exception is not ApiException)
			{
				if (context.Exception is InvalidOperationException
					|| context.Exception is ValidationException
					|| context.Exception is DbUpdateConcurrencyException)
				{
					context.Exception = new ApiException(context.Exception.Message, context.Exception);
				}
				else
				{
					context.Exception = new ApiException(context.Exception.Message, context.Exception, HttpStatusCode.InternalServerError);
				}
			}

			context.Result = new ObjectResult(context.Exception.Flatten())
			{
				StatusCode = context.Exception is ApiException apiException
					? (int)apiException.StatusCode
					: (int)HttpStatusCode.InternalServerError
			};

			_logger.Error(context.Exception, string.Empty);
			context.ExceptionHandled = true;
		}
	}
}
