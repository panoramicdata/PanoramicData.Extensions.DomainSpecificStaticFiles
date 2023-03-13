namespace PanoramicData.Extensions.DomainSpecificStaticFiles;

/// <summary>
/// The definition of a resource provider that can be used by this subsystem
/// to return a resource for a given request
/// </summary>
public interface IResourceProvider
{
	/// <summary>
	/// Get the information about a resource, such as whether it exists
	/// </summary>
	/// <param name="resourcePath">The relative path to the resource</param>
	/// <param name="cancellationToken">The token used to manage cancellation</param>
	/// <returns>ResourceInfo about the requested resource</returns>
	Task<ResourceInfo> GetResourceInfoAsync(string resourcePath, CancellationToken cancellationToken);

	/// <summary>
	/// Return the contents of a resource
	/// </summary>
	/// <param name="resourceInfo">The resource for which the contents are required</param>
	/// <param name="cancellationToken">The token used to manage cancellation</param>
	/// <returns>A ResourceContentsAccessor instance providing access to the resource contents</returns>
	Task<ResourceContentAccessor> GetResourceContentAccessorAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken);

	/// <summary>
	/// Return the stream giving access to the contents of a resource
	/// </summary>
	/// <param name="resourceInfo">The resource for which the contents are required</param>
	/// <param name="cancellationToken">The token used to manage cancellation</param>
	/// <returns>A Stream instance providing access to the resource contents</returns>
	Task<Stream> GetResourceContentStreamAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken);

	/// <summary>
	/// Get the hash of the resource contents
	/// </summary>
	/// <param name="resourceInfo">The resource for which a hash is required</param>
	/// <param name="cancellationToken">The token used to manage cancellation</param>
	/// <returns>A hash of the contents of the resource</returns>
	Task<byte[]> GetResourceContentsHashAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken);

	/// <summary>
	/// Get the last modified date and time of the resource contents
	/// </summary>
	/// <param name="resourceInfo">The resource for which a hash is required</param>
	/// <param name="cancellationToken">The token used to manage cancellation</param>
	/// <returns>The last modified date and time of the resource</returns>
	Task<DateTime> GetResourceLastModifiedUtcAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken);
}