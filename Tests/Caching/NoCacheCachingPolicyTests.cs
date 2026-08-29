using AwesomeAssertions;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Caching;

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
