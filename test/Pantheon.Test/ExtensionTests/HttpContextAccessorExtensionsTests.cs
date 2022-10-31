using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using Pantheon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace Pantheon.Test.ExtensionTests
{
	public class HttpContextAccessorExtensionsTests
	{
		[Fact]
		public void GetHeaderValue_Should_Return_Value_If_Header_Exists()
		{
			StringValues expectedHeaderValue = new("5");

			var headers = new HeaderDictionary(new Dictionary<string, StringValues>
			{
				{ "TestHeader", expectedHeaderValue}
			}) as IHeaderDictionary;

			var request = new Mock<HttpRequest>();
			request.Setup(x => x.Headers).Returns(headers);

			var httpContext = new Mock<HttpContext>();
			httpContext.Setup(x => x.Request).Returns(request.Object);

			var accessor = new Mock<IHttpContextAccessor>();
			accessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

			int actualHeaderValue = accessor.Object.GetHeaderValue("TestHeader", 0);
			actualHeaderValue.Should().Be(int.Parse(expectedHeaderValue.FirstOrDefault()!));
		}

		[Fact]
		public void GetHeaderValue_Should_Return_DefaultValue_If_Header_Doesnt_Exist()
		{
			var headers = new HeaderDictionary(new Dictionary<string, StringValues>
			{
				{ "TestHeader", "5"}
			}) as IHeaderDictionary;

			var request = new Mock<HttpRequest>();
			request.Setup(x => x.Headers).Returns(headers);

			var httpContext = new Mock<HttpContext>();
			httpContext.Setup(x => x.Request).Returns(request.Object);

			var accessor = new Mock<IHttpContextAccessor>();
			accessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

			int actualHeaderValue = accessor.Object.GetHeaderValue("NonExistingHeader", 0);
			actualHeaderValue.Should().Be(0);
		}

		[Fact]
		public void GetHeaderValue_Should_Return_DefaultValue_If_HtppContext_Is_Null()
		{
			var headers = new HeaderDictionary(new Dictionary<string, StringValues>
			{
				{ "TestHeader", "5"}
			}) as IHeaderDictionary;

			var request = new Mock<HttpRequest>();
			request.Setup(x => x.Headers).Returns(headers);

			HttpContext? httpContext = null;

			var accessor = new Mock<IHttpContextAccessor>();
			accessor.Setup(x => x.HttpContext).Returns(httpContext);

			int actualHeaderValue = accessor.Object.GetHeaderValue("TestHeader", 0);
			actualHeaderValue.Should().Be(0);
		}

		[Fact]
		public void GetClaimValue_Should_Return_Value_If_Claim_Exists()
		{
			Guid sid = Guid.NewGuid();

			List<Claim> claims = new()
			{
				new(ClaimTypes.Sid, sid.ToString())
			};

			var httpContext = new Mock<HttpContext>();
			httpContext.Setup(x => x.User.Claims).Returns(claims);

			var accessor = new Mock<IHttpContextAccessor>();
			accessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

			Guid actualClaimlValue = accessor.Object.GetClaimValue(ClaimTypes.Sid, Guid.Empty);
			actualClaimlValue.Should().Be(sid);
		}

		[Fact]
		public void GetClaimValue_Should_Return_DefaultValue_If_Claim_Doesnt_Exist()
		{
			Guid sid = Guid.NewGuid();

			List<Claim> claims = new()
			{
				new(ClaimTypes.Sid, sid.ToString())
			};

			var httpContext = new Mock<HttpContext>();
			httpContext.Setup(x => x.User.Claims).Returns(claims);

			var accessor = new Mock<IHttpContextAccessor>();
			accessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

			Guid actualClaimlValue = accessor.Object.GetClaimValue("NonExistingClaim", Guid.Empty);
			actualClaimlValue.Should().Be(Guid.Empty);
		}

		[Fact]
		public void GetClaimValue_Should_Return_DefaultValue_If_HttpContext_Is_Null()
		{
			Guid sid = Guid.NewGuid();

			List<Claim> claims = new()
			{
				new(ClaimTypes.Sid, sid.ToString())
			};

			HttpContext? httpContext = null;

			var accessor = new Mock<IHttpContextAccessor>();
			accessor.Setup(x => x.HttpContext).Returns(httpContext);

			Guid actualClaimlValue = accessor.Object.GetClaimValue(ClaimTypes.Sid, Guid.Empty);
			actualClaimlValue.Should().Be(Guid.Empty);
		}
	}
}
