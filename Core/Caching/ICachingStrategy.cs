namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

/// <summary>
/// Marker interface for all caching strategy implementations
/// </summary>
public interface ICachingStrategy
{
	/// <summary>
	/// Write the cache headers appropriate to the strategy in use
	/// </summary>
	/// <param name="interceptionContext">The interception context for the request</param>
	/// <param name="resourceContentAccessor">The accessor used to gain access to the resource</param>
	void WriteCacheHeaders(IInterceptionContext interceptionContext, ResourceContentAccessor resourceContentAccessor);
}
