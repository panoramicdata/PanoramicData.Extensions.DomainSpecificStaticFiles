using Microsoft.Extensions.Options;
using PanoramicData.Extensions.DomainMappedStaticFiles;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for the IServiceCollection interface
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Add the types necessary to serve static whitelabelling files
	/// </summary>
	/// <param name="services">The service collection instance on which we are to operate</param>
	/// <param name="requestMappings">The request mappings to be used in whitelabelling</param>
	/// <returns>The instance being extended, to support method chaining</returns>
	public static IServiceCollection AddDomainMappedStaticFiles(this IServiceCollection services, IConfiguration configuration, IEnumerable<RequestMapping> requestMappings)
		=> AddWhitelabellingStaticFiles(services, configuration, t =>
		{
			if (!t.RequestMappings.Any())
			{
				t.RequestMappings = requestMappings;
			}
		});

	/// <summary>
	/// Add the types necessary to serve static whitelabelling files
	/// </summary>
	/// <param name="services">The service collection instance on which we are to operate</param>
	/// <param name="config">The configuration action to be applied to the options that are to be used for whitelabelling</param>
	/// <returns>The instance being extended, to support method chaining</returns>
	public static IServiceCollection AddWhitelabellingStaticFiles(this IServiceCollection services, IConfiguration configuration, Action<DomainMappedStaticFileOptions> config)
	{
		_ = services.AddOptions<DomainMappedStaticFileOptions>().Bind(configuration.GetSection("Whitelabelling")).Configure(config);
		_ = services.AddTransient(CreateResourceProvider);
		_ = services.AddTransient<DomainMappedStaticFileMiddleware>();
		_ = services.AddTransient<DomainMappedStaticFileRequestHandler>();

		return services;
	}

	private static IResourceProvider CreateResourceProvider(IServiceProvider serviceProvider)
	{
		var options = serviceProvider.GetRequiredService<IOptions<DomainMappedStaticFileOptions>>().Value;
		return (IResourceProvider)ActivatorUtilities.CreateInstance(serviceProvider, options.ResourceProviderType);
	}
}
