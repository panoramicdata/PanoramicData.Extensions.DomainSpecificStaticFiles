namespace Benchmarks;

public class RequestMapping
{
	public required string RequestPathPrefix { get; set; }

	public required string ResourcePath { get; set; }

	public TimeSpan? OverrideCachingPeriod { get; set; }
}