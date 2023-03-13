namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

public class MaxAgeCachingPolicy : ICachingPolicy
{
	public MaxAgeCachingPolicy()
	{

	}

	public MaxAgeCachingPolicy(TimeSpan cachingPeriod)
	{
		CachingPeriod = cachingPeriod;
	}

	public TimeSpan CachingPeriod { get; set; } = TimeSpan.FromDays(7);

	public ICachingStrategy CreateCachingStrategy() => new MaxAgeCachingStrategy(this);
}
