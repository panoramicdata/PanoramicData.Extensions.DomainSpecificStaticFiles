using AwesomeAssertions;
using Microsoft.Extensions.Primitives;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Caching;

public class NoCacheCachingStrategyTests
{
	[Fact]
	public void WriteCacheHeaders_UsingDefaultPolicy_SetsNoCacheControlHeader()
	{
		var result = CachingStrategyHarness.WriteCacheControlHeader(new NoCacheCachingPolicy());

		result.Should().Equal((StringValues)"no-cache");
	}
}
