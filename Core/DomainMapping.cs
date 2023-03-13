using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles;

public class DomainMapping
{
	/// <summary>
	/// Create a new domain mapping with no values provided
	/// </summary>
	public DomainMapping() { }

	/// <summary>
	/// Create a new domain mapping with the specified domain and resource container
	/// </summary>
	/// <param name="domain">The domain for which the mapping is to apply</param>
	/// <param name="resourceSubContainer">The subcontainer that the resources for this domain are served from (e.g. the folder)</param>
	/// <remarks>Note that the resource container is usually case-sensitive, especially on Linux</remarks>
	public DomainMapping(string domain, string resourceSubContainer)
	{
		Domain = domain;
		ResourceSubPath = resourceSubContainer;
	}

	/// <summary>
	/// The domain being mapped
	/// </summary>
	public required string Domain { get; set; } = string.Empty;

	/// <summary>
	/// The subpath (such as the name of a folder) in which the resources for this domain reside
	/// </summary>
	/// <remarks>
	/// Note that this is usually case-sensitive, especially on Linux
	/// </remarks>
	public required string ResourceSubPath { get; set; } = string.Empty;

	/// <summary>
	/// An override for a custom caching policy for resources matching this mapping
	/// </summary>
	public ICachingPolicy? OverrideCachingPolicy { get; set; }

}
