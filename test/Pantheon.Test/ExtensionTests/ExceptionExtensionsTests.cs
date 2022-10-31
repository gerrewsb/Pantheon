using FluentAssertions;
using Pantheon.Extensions;
using System;
using Xunit;

namespace Pantheon.Test.ExtensionTests
{
	public class ExceptionExtensionsTests
	{
		[Fact]
		public void Flatten_Should_Concat_All_InnerExceptions()
		{
			Exception exception = new("Exception", new("InnerException1", new("InnerException2")));
			string? flatExceptionMessage = exception.AggregateExceptionMessages();
			flatExceptionMessage.Should().NotBeNullOrWhiteSpace();
			flatExceptionMessage.Should().Be("Exception\r\nInnerException1\r\nInnerException2\r\n");
		}

		[Fact]
		public void Flatten_Without_InnerExceptions_Should_Return_Message_Of_Exception()
		{
			Exception exception = new("Exception");
			string? flatExceptionMessage = exception.AggregateExceptionMessages();
			flatExceptionMessage.Should().NotBeNullOrWhiteSpace();
			flatExceptionMessage.Should().Be("Exception\r\n");
		}

		[Fact]
		public void Flatten_Should_Return_Null_If_Exception_Is_Null()
		{
			Exception? exception = null;
			string? flatExceptionMessage = exception?.AggregateExceptionMessages();
			flatExceptionMessage.Should().BeNull();
		}
	}
}
