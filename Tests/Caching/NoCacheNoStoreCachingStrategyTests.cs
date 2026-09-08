using AwesomeAssertions;
using Microsoft.Extensions.Primitives;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Caching;

public class NoCacheNoStoreCachingStrategyTests
{
	[Fact]
	public void WriteCacheHeaders_UsingDefaultPolicy_SetsNoCacheNoStoreControlHeader()
	{
		var result = CachingStrategyHarness.WriteCacheControlHeader(new NoCacheNoStoreCachingPolicy());

		result.Should().Equal((StringValues)"no-cache, no-store");
	}
}
