using FluentAssertions;
using Pantheon.Extensions;
using System;
using Xunit;

namespace Pantheon.Test.ExtensionTests
{
	public class StringExtensionsTests
	{
		[Theory]
		[InlineData("7653.23")]
		[InlineData("7653,23")]
		public void ConvertValue_Should_Convert_To_Typed_Value_Invariant_Culture(string decimalValue)
		{
			decimal convertedValue = decimalValue.ConvertToValue(0m);
			convertedValue.Should().Be(7653.23m);
		}


		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void ConvertValue_Should_Return_DefaultValue_When_String_Is_Null_Empty_WhiteSpace(string? value)
		{
			Guid convertedValue = value.ConvertToValue(Guid.Empty);
			convertedValue.Should().Be(Guid.Empty);
		}

		[Fact]
		public void ConvertValue_Should_Return_DefaultValue_If_Conversion_Failed()
		{
			string value = "350";
			Guid convertedValue = value.ConvertToValue(Guid.Empty);
			convertedValue.Should().Be(Guid.Empty);
		}
	}
}
