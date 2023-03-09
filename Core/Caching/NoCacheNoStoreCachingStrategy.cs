namespace PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

/// <summary>
/// The strategy that applies the no cache, no store caching policy
/// </summary>
internal class NoCacheNoStoreCachingStrategy : ICachingStrategy
{
	/// <inheritdoc/>
	public void WriteCacheHeaders(IInterceptionContext interceptionContext, ResourceContentAccessor resourceContentAccessor)
		=> interceptionContext.HttpContext.Response.Headers.CacheControl =
			(Microsoft.Extensions.Primitives.StringValues)"no-cache, no-store";
}