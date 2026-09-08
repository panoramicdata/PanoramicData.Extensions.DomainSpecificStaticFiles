using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles;

/// <summary>
/// A mapping from a request URL prefix to the location of the resources that serve it
/// </summary>
public class ResourceMapping
{
	/// <summary>
	/// The prefix of a request that is to be mapped to a resource
	/// </summary>
	public required string RequestUrlPrefix { get; set; }

	/// <summary>
	/// The location of the resources that are to be delivered
	/// </summary>
	public required string ResourcePath { get; set; }

	/// <summary>
	/// The caching policy to be applied to these resources
	/// </summary>
	public required ICachingPolicy CachingPolicy { get; set; }
}