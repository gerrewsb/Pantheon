using FluentAssertions;
using Pantheon.Extensions;
using Xunit;

namespace Pantheon.Test.ExtensionTests
{
	public class ApiClientExtensionsTests
	{
		[Fact]
		public void CheckBaseUrl_Without_ApiSuffixCheck_Url_Without_Backslash_At_End_Should_Append_Backslash()
		{
			string url = "www.google.be";
			string? checkUrl = ApiClientExtensions.CheckBaseUrl(url, false);
			checkUrl.Should().NotBeNullOrWhiteSpace();
			checkUrl.Should().Be("www.google.be/");
		}

		[Fact]
		public void CheckBaseUrl_With_ApiSuffixCheck_Should_Append_ApiSuffix_Url_Without_Backslash_At_End_Should_Append_Backslash()
		{
			string url = "www.google.be";
			string? checkUrl = ApiClientExtensions.CheckBaseUrl(url, true);
			checkUrl.Should().NotBeNullOrWhiteSpace();
			checkUrl.Should().Be("www.google.be/api/");
		}

		[Fact]
		public void CheckBaseUrl_Url_With_Api_At_The_End_Should_Remove_Api_Without_ApiSuffixCheck_Url_Without_Backslash_End_Should_Append_Backslash()
		{
			string url = "www.google.be/api";
			string? checkUrl = ApiClientExtensions.CheckBaseUrl(url, false);
			checkUrl.Should().NotBeNullOrWhiteSpace();
			checkUrl.Should().Be("www.google.be/");
		}

		[Fact]
		public void CheckBaseUrl_Url_With_Api_At_The_End_Should_Keep_Api_With_ApiSuffixCheck_Url_Without_Backslash_End_Should_Append_Backslash()
		{
			string url = "www.google.be/api";
			string? checkUrl = ApiClientExtensions.CheckBaseUrl(url, true);
			checkUrl.Should().NotBeNullOrWhiteSpace();
			checkUrl.Should().Be("www.google.be/api/");
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void CheckBaseUrl_With_BaseUrl_Null_Empty_WhiteSpace_Should_Return_null(string? baseUrl)
		{
			string? checkUrl = ApiClientExtensions.CheckBaseUrl(baseUrl);
			checkUrl.Should().BeNullOrWhiteSpace();
		}
	}
}
