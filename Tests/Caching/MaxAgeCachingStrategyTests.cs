using AwesomeAssertions;
using Microsoft.Extensions.Primitives;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Caching;

public class MaxAgeCachingStrategyTests
{
	[Fact]
	public void WriteCacheHeaders_UsingDefaultPolicy_SetsMaxAgeCacheControlHeader()
	{
		var result = CachingStrategyHarness.WriteCacheControlHeader(new MaxAgeCachingPolicy());

		result.Should().Equal((StringValues)"max-age=604800");
	}

	[Fact]
	public void WriteCacheHeaders_UsingCustom1DayPolicy_SetsMaxAgeCacheControlHeader()
	{
		var result = CachingStrategyHarness.WriteCacheControlHeader(new MaxAgeCachingPolicy(TimeSpan.FromDays(1)));

		result.Should().Equal((StringValues)"max-age=86400");
	}
}
