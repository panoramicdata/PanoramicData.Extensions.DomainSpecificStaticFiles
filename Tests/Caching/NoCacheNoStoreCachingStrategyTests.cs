using FluentAssertions;
using Microsoft.Extensions.Primitives;
using PanoramicData.Extensions.DomainMappedStaticFiles.Caching;
using PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Caching;

public class NoCacheNoStoreCachingStrategyTests
{
	[Fact]
	public void WriteCacheHeaders_UsingDefaultPolicy_SetsNoCacheNoStoreControlHeader()
	{
		var policy = new NoCacheNoStoreCachingPolicy();
		var strategy = policy.CreateCachingStrategy();
		var configuration = DomainMappedStaticFileConfigurationFactory.CreateConfiguration(new List<(string, ResourceMapping)>
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
