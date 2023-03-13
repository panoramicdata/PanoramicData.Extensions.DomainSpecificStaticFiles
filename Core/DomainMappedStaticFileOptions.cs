using PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// Options for the DomainMappedStaticFileMiddleware
/// </summary>
public class DomainMappedStaticFileOptions
{
	/// <summary>
	/// The mappings for requests that we are to intercept
	/// </summary>
	public IList<RequestMapping> RequestMappings { get; set; } = new List<RequestMapping>();

	/// <summary>
	/// The default caching policy that is to be applied to responses
	/// </summary>
	/// <remarks>A value of zero can be used to disable caching headers</remarks>
	public ICachingPolicy DefaultCachingPolicy { get; set; } = new MaxAgeCachingPolicy(TimeSpan.FromDays(7));

	/// <summary>
	/// The type used to access and serve resources
	/// </summary>
	internal Type ResourceProviderType { get; set; } = typeof(PhysicalFileResourceProvider);

	/// <summary>
	/// Register an alternative resource provider for accessing resources to be served
	/// </summary>
	/// <typeparam name="T">The type that is to be used to access resources</typeparam>
	public void RegisterFileProvider<T>() where T : class, IResourceProvider
		=> ResourceProviderType = typeof(T);
}