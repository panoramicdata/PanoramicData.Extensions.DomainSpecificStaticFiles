namespace PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

/// <summary>
/// The strategy that applies the max age caching policy
/// </summary>
internal class MaxAgeCachingStrategy : ICachingStrategy
{
	private readonly MaxAgeCachingPolicy _cachingPolicy;

	public MaxAgeCachingStrategy(ICachingPolicy cachingPolicy)
	{
		if (cachingPolicy is not MaxAgeCachingPolicy maxAgeCachingPolicy)
		{
			throw new ArgumentException("Incorrect caching policy provided!");
		}

		_cachingPolicy = maxAgeCachingPolicy;
	}

	/// <inheritdoc/>
	public void WriteCacheHeaders(IInterceptionContext interceptionContext, ResourceContentAccessor resourceContentAccessor)
	{
		// Add caching headers
		var cachingPeriodSeconds = _cachingPolicy.CachingPeriod.TotalSeconds;
		interceptionContext.Response.Headers.CacheControl = cachingPeriodSeconds > 0
			? (Microsoft.Extensions.Primitives.StringValues)$"max-age={cachingPeriodSeconds:F0}"
			: (Microsoft.Extensions.Primitives.StringValues)"no-cache";
	}
}
