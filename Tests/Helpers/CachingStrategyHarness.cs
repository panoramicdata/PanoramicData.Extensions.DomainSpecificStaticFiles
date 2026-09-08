using Microsoft.Extensions.Primitives;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

/// <summary>
/// Applies a caching policy's strategy to a fake request and reports the Cache-Control header it
/// wrote. Every caching strategy behaves the same way apart from that header, so the configuration,
/// interception context and content accessor they all need are built once here.
/// </summary>
internal static class CachingStrategyHarness
{
	public static StringValues WriteCacheControlHeader(ICachingPolicy policy)
	{
		var configuration = DomainSpecificStaticFileConfigurationFactory
			.CreateSingleMappingConfiguration("style", "Style", policy);
		var context = FakeInterceptionContextBuilder
			.CreateBuilder()
			.WithConfiguration(configuration)
			.Build();

		policy
			.CreateCachingStrategy()
			.WriteCacheHeaders(context, ResourceContextAccessorFactory.CreateDefaultContentAccessor());

		return context.Response.Headers.CacheControl;
	}
}
