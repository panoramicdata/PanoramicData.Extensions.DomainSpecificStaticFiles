using FluentAssertions;
using PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Caching;

public class NoCacheNoStoreCachingPolicyTests
{
	[Fact]
	public void CreateCachingStrategy_RequestStrategy_ReturnsNoCacheNoStoreCachingStrategy()
	{
		var policy = new NoCacheNoStoreCachingPolicy();
		var result = policy.CreateCachingStrategy();

		result.Should().BeOfType<NoCacheNoStoreCachingStrategy>();
	}
}
