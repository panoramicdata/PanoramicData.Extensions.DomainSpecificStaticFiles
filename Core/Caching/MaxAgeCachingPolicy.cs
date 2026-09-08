namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

/// <summary>
/// A caching policy that instructs clients to cache a resource for a fixed period
/// </summary>
public class MaxAgeCachingPolicy : ICachingPolicy
{
	/// <summary>
	/// Create a new policy using the default caching period of seven days
	/// </summary>
	public MaxAgeCachingPolicy()
	{

	}

	/// <summary>
	/// Create a new policy using the specified caching period
	/// </summary>
	/// <param name="cachingPeriod">The period for which clients may cache the resource</param>
	public MaxAgeCachingPolicy(TimeSpan cachingPeriod)
	{
		CachingPeriod = cachingPeriod;
	}

	/// <summary>
	/// The period for which clients may cache the resource
	/// </summary>
	public TimeSpan CachingPeriod { get; set; } = TimeSpan.FromDays(7);

	/// <inheritdoc />
	public ICachingStrategy CreateCachingStrategy() => new MaxAgeCachingStrategy(this);
}
