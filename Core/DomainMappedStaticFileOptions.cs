namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// Options for the DomainMappedStaticFileMiddleware
/// </summary>
public class DomainMappedStaticFileOptions
{
	/// <summary>
	/// The mappings for requests that we are to intercept
	/// </summary>
	public IEnumerable<RequestMapping> RequestMappings { get; set; } = Enumerable.Empty<RequestMapping>();

	/// <summary>
	/// The period for which to cache requests, in minutes
	/// </summary>
	/// <remarks>A value of zero can be used to disable caching headers</remarks>
	public TimeSpan DefaultCachingPeriod { get; set; } = TimeSpan.FromDays(30);

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