using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;

internal class FakeHttpContextFactory
{
	internal static HttpContext CreateDefaultHttpContext()
		=> new DefaultHttpContext();
}