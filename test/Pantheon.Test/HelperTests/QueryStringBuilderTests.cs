using FluentAssertions;
using Pantheon.Helpers;
using System;
using System.Collections.Generic;
using System.Web;
using Xunit;

namespace Pantheon.Test.HelperTests
{
	public class QueryStringBuilderTests
	{
		[Fact]
		public void Add_KeyValue_Should_Add_One_Parameter_To_QueryString()
		{
			var builder = new QueryStringBuilder();
			builder.Add("TestParameter", "TestValue");
			builder.GetUrlExtension().Should().Be("?TestParameter=TestValue");
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData(" ")]
		public void Add_KeyValue_With_Value_Null_Empty_WhiteSpace_Should_Not_Add_Parameter_To_QueryString(object? value)
		{
			var builder = new QueryStringBuilder();
			builder.Add("TestParameter", value);
			builder.GetUrlExtension().Should().Be(string.Empty);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData(" ")]
		public void Add_KeyValue_With_Key_Null_Empty_WhiteSpace_Should_Not_Add_Parameter_To_QueryString(string key)
		{
			var builder = new QueryStringBuilder();
			builder.Add(key, "TestValue");
			builder.GetUrlExtension().Should().Be(string.Empty);
		}

		[Fact]
		public void Add_KeyValue_With_DateTime_Value_Should_Add_DateTime_Parameter_In_Correct_Format()
		{
			DateTime dateTimeParameter = new(1991, 11, 21, 10, 55, 3);
			var builder = new QueryStringBuilder();
			builder.Add("DateTimeParameter", dateTimeParameter);
			builder.GetUrlExtension().Should().Be($"?DateTimeParameter={dateTimeParameter:yyyy-MM-ddTHH:mm:ssZ}");
		}

		[Fact]
		public void Add_Multiple_KeyValue_Should_Add_Multiple_Parameters_To_QueryString()
		{
			DateTime dateTimeParameter = new(1991, 11, 21, 10, 55, 3);
			var builder = new QueryStringBuilder();
			builder.Add("StringParameter", "ValueOfString")
				.Add(nameof(dateTimeParameter), dateTimeParameter)
				.Add("NumericParameter", 5);
			builder.GetUrlExtension().Should().Be($"?StringParameter=ValueOfString&{nameof(dateTimeParameter)}={dateTimeParameter:yyyy-MM-ddTHH:mm:ssZ}&NumericParameter=5");
		}

		[Fact]
		public void Add_KeyValue_StringValue_Should_Encode_Value()
		{
			var builder = new QueryStringBuilder();
			builder.Add("StringParameter", "ValueOfString+/");
			builder.GetUrlExtension().Should().Be($"?StringParameter={HttpUtility.UrlEncodeUnicode("ValueOfString+/")}");
		}

		[Fact]
		public void Add_Multiple_KeyValue_Should_Only_Add_Valid_Parameters_To_QueryString()
		{
			DateTime dateTimeParameter = new(1991, 11, 21, 10, 55, 3);
			var builder = new QueryStringBuilder();
			builder.Add("StringParameter", "ValueOfString")
				.Add("InvalidParameter", null)
				.Add(nameof(dateTimeParameter), dateTimeParameter)
				.Add("NumericParameter", 5);
			builder.GetUrlExtension().Should().Be($"?StringParameter=ValueOfString&{nameof(dateTimeParameter)}={dateTimeParameter:yyyy-MM-ddTHH:mm:ssZ}&NumericParameter=5");
		}

		[Fact]
		public void Add_Dictionary_Should_Add_Parameters_To_QueryString()
		{
			DateTime dateTimeParameter = new(1991, 11, 21, 10, 55, 3);

			Dictionary<string, object?> parameterDictionary = new()
			{
				{ "Parameter1", "SomeValue" },
				{ "Parameter2", 5 },
				{ "Parameter3", dateTimeParameter }
			};

			var builder = new QueryStringBuilder().Add(parameterDictionary);
			builder.GetUrlExtension().Should().Be($"?Parameter1=SomeValue&Parameter2=5&Parameter3={dateTimeParameter:yyyy-MM-ddTHH:mm:ssZ}");
		}

		[Fact]
		public void Add_Dictionary_Should_Only_Add_Valid_Parameters_To_QueryString()
		{
			DateTime dateTimeParameter = new(1991, 11, 21, 10, 55, 3);

			Dictionary<string, object?> parameterDictionary = new()
			{
				{ "Parameter1", "SomeValue" },
				{ "InvalidParameter", null },
				{ "Parameter2", 5 },
				{ "", "ValueWithInvalidKey" },
				{ "Parameter3", dateTimeParameter }
			};

			var builder = new QueryStringBuilder().Add(parameterDictionary);
			builder.GetUrlExtension().Should().Be($"?Parameter1=SomeValue&Parameter2=5&Parameter3={dateTimeParameter:yyyy-MM-ddTHH:mm:ssZ}");
		}

		[Fact]
		public void Add_Dictionary_StringValue_Should_Encode_Value()
		{
			Dictionary<string, object?> parameterDictionary = new()
			{
				{ "Parameter1", "ValueOfString+/" },
				{ "Parameter2", 5 },
			};

			var builder = new QueryStringBuilder().Add(parameterDictionary);
			builder.GetUrlExtension().Should().Be($"?Parameter1={HttpUtility.UrlEncodeUnicode("ValueOfString+/")}&Parameter2=5");
		}

		[Fact]
		public void Add_Model_Should_Add_All_Properties_Of_Model_To_QueryString()
		{
			DateTime dateTimeParameter = new(1991, 11, 21, 10, 55, 3);

			TestModel model = new()
			{
				ID = 5,
				Name = "TestModel",
				CreationDate = dateTimeParameter,
				Location = "België"
			};

			var builder = new QueryStringBuilder().Add(model);
			builder.GetUrlExtension().Should().Be($"?ID=5&Name=TestModel&CreationDate={dateTimeParameter:yyyy-MM-ddTHH:mm:ssZ}&Location={HttpUtility.UrlEncodeUnicode("België")}");
		}

		[Fact]
		public void Add_Model_Should_Add_Valid_Properties_Of_Model_To_QueryString()
		{
			DateTime dateTimeParameter = new(1991, 11, 21, 10, 55, 3);

			TestModel model = new()
			{
				ID = 5,
				Name = "TestModel",
				CreationDate = dateTimeParameter,
				Location = null
			};

			var builder = new QueryStringBuilder().Add(model);
			builder.GetUrlExtension().Should().Be($"?ID=5&Name=TestModel&CreationDate={dateTimeParameter:yyyy-MM-ddTHH:mm:ssZ}");
		}

		[Fact]
		public void Add_Model_With_StringValue_Shoud_Encode_Value()
		{
			var model = new { Location = "België" };
			var builder = new QueryStringBuilder().Add(model);
			builder.GetUrlExtension().Should().Be($"?Location={HttpUtility.UrlEncodeUnicode("België")}");
		}

		private class TestModel
		{
			public int ID { get; set; }
			public string? Name { get; set; }
			public DateTime CreationDate { get; set; }
			public string? Location { get; set; }
		}
	}
}
