using FluentAssertions;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Caching;

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
