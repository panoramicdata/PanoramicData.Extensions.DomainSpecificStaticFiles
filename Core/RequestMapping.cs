using PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// A mapping from a request prefix to a resource location
/// </summary>
public class RequestMapping
{
	/// <summary>
	/// The prefix of a request that is to be mapped to a resource
	/// </summary>
	public string RequestUrlPrefix { get; set; } = string.Empty;

	/// <summary>
	/// The location of the resources that are to be delivered
	/// </summary>
	public string ResourcePath { get; set; } = string.Empty;

	/// <summary>
	/// An override for a custom caching policy for this resource
	/// </summary>
	public ICachingPolicy? OverrideCachingPolicy { get; set; }
}
