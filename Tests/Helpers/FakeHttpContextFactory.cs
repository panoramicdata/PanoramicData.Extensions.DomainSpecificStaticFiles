using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

internal static class FakeHttpContextFactory
{
	internal static HttpContext CreateDefaultHttpContext()
		=> new DefaultHttpContext();
}