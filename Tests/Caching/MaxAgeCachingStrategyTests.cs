using FluentAssertions;
using Microsoft.Extensions.Primitives;
using PanoramicData.Extensions.DomainMappedStaticFiles.Caching;
using PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Caching;

public class MaxAgeCachingStrategyTests
{
	[Fact]
	public void WriteCacheHeaders_UsingDefaultPolicy_SetsMaxAgeCacheControlHeader()
	{
		var policy = new MaxAgeCachingPolicy();
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

		context.Response.Headers.CacheControl.Should().Equal((StringValues)"max-age=604800");
	}

	[Fact]
	public void WriteCacheHeaders_UsingCustom1DayPolicy_SetsMaxAgeCacheControlHeader()
	{
		var policy = new MaxAgeCachingPolicy(TimeSpan.FromDays(1));
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

		context.Response.Headers.CacheControl.Should().Equal((StringValues)"max-age=86400");
	}
}
