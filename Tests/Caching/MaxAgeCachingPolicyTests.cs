using AwesomeAssertions;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Caching;

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
