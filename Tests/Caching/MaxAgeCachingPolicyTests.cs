using FluentAssertions;
using PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Caching;

public class MaxAgeCachingPolicyTests
{
	[Fact]
	public void CreateCachingStrategy_RequestStrategy_ReturnsMaxAgeCachingStrategy()
	{
		var policy = new MaxAgeCachingPolicy();
		var result = policy.CreateCachingStrategy();

		result.Should().BeOfType<MaxAgeCachingStrategy>();
	}
}
