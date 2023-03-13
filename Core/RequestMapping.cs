using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles;

/// <summary>
/// A mapping from a request prefix to a resource location
/// </summary>
public class RequestMapping
{
	public RequestMapping() { }

	/// <summary>
	/// Create a new request mapping with the specified Url prefix and resource path prefix
	/// </summary>
	/// <param name="relativeUrlPrefix">The relative prefix that is to be intercepted</param>
	/// <param name="resourcePathPrefix">The container in which all Domain-specific resources reside</param>
	/// <remarks>The resource path prefix is usually case-sensitive, especially on Linux</remarks>
	public RequestMapping(string relativeUrlPrefix, string resourcePathPrefix)
	{
		RelativeUrlPrefix = relativeUrlPrefix;
		ResourcePathPrefix = resourcePathPrefix;
	}

	/// <summary>
	/// The prefix of a request that is to be mapped to a resource
	/// </summary>
	public string RelativeUrlPrefix { get; set; } = string.Empty;

	/// <summary>
	/// The outermost location of the resources that are to be delivered
	/// </summary>
	/// <remarks>
	/// Note that this is usually case-sensitive, especially on Linux
	/// </remarks>
	public string ResourcePathPrefix { get; set; } = string.Empty;

	/// <summary>
	/// An override for a custom caching policy for this resource
	/// </summary>
	public ICachingPolicy? OverrideCachingPolicy { get; set; }

	/// <summary>
	/// The mappings between domains and resource paths
	/// </summary>
	public List<DomainMapping> DomainMappings { get; set; } = new();
}
