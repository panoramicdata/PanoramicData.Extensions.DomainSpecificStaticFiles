namespace PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

/// <summary>
/// Marker interface for all caching policies
/// </summary>
public interface ICachingPolicy
{
	/// <summary>
	/// The factory method to use for creating the caching strategy that
	/// applies the relevant caching policy
	/// </summary>
	/// <returns>An instance of the strategy that applies the appropriate cache control headers</returns>
	ICachingStrategy CreateCachingStrategy();
}
