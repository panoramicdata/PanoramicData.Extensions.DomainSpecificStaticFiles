namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// The definition of a resource provider that can be used by the white labelling subsystem
/// to return a resource for a given request
/// </summary>
public interface IResourceProvider
{
	/// <summary>
	/// Get the information about a resource, such as whether it exists
	/// </summary>
	/// <param name="resourcePath">The relative path to the resource</param>
	/// <returns>ResourceInfo about the requested resource</returns>
	Task<ResourceInfo> GetResourceInfoAsync(string resourcePath);

	/// <summary>
	/// Return the contents of a resource
	/// </summary>
	/// <param name="resourceInfo">The resource for which the contents are required</param>
	/// <returns>A ResourceContents class containing the resource contents</returns>
	Task<ResourceContentAccessor> GetResourceContentAccessorAsync(ResourceInfo resourceInfo);
}