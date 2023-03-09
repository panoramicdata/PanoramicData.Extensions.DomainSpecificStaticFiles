namespace PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

/// <summary>
/// The strategy that applies the no cache caching policy
/// </summary>
public class NoCacheCachingPolicy : ICachingPolicy
{
	/// <summary>
	/// Create an instance of the strategy that applies this policy
	/// </summary>
	/// <returns>An instance of the caching strategy that applies the appropriate cache control headers</returns>
	public ICachingStrategy CreateCachingStrategy() => new NoCacheCachingStrategy();
}
