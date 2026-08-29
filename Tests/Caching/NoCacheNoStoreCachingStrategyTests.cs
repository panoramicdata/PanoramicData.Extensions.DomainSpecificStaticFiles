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
		var policy = new NoCacheNoStoreCachingPolicy();
		var strategy = policy.CreateCachingStrategy();
		var configuration = DomainSpecificStaticFileConfigurationFactory.CreateConfiguration(new List<(string, ResourceMapping)>
		{
			new ("localhost",
				new ResourceMapping
				{
					RequestUrlPrefix = "style",
					ResourcePath = "Style",
					CachingPolicy = policy
				})
		});
		var context = FakeInterceptionContextBuilder
			.CreateBuilder()
			.WithConfiguration(configuration)
			.Build();
		var accessor = ResourceContextAccessorFactory.CreateDefaultContentAccessor();
		strategy.WriteCacheHeaders(context, accessor);

		context.Response.Headers.CacheControl.Should().Equal((StringValues)"no-cache, no-store");
	}
}
