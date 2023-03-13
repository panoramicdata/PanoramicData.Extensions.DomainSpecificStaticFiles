using FluentAssertions;
using PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Caching;

public class NoCacheCachingPolicyTests
{
	[Fact]
	public void CreateCachingStrategy_RequestStrategy_ReturnsNoCacheCachingStrategy()
	{
		var policy = new NoCacheCachingPolicy();
		var result = policy.CreateCachingStrategy();

		result.Should().BeOfType<NoCacheCachingStrategy>();
	}
}
