namespace PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

/// <summary>
/// The strategy that applies the no cache caching policy
/// </summary>
internal class NoCacheCachingStrategy : ICachingStrategy
{
	/// <inheritdoc/>
	public void WriteCacheHeaders(IInterceptionContext interceptionContext, ResourceContentAccessor resourceContentAccessor)
		=> interceptionContext.Response.Headers.CacheControl =
			(Microsoft.Extensions.Primitives.StringValues)"no-cache";
}