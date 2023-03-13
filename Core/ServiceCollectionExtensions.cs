using PanoramicData.Extensions.DomainSpecificStaticFiles;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for the IServiceCollection interface
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Add the types necessary to serve static domain-specific files
	/// </summary>
	/// <param name="services">The service collection instance on which we are to operate</param>
	/// <param name="config">The configuration action to be applied to the options that are to be used for serving resources</param>
	/// <returns>The instance being extended, to support method chaining</returns>
	public static IServiceCollection AddDomainSpecificStaticFiles(this IServiceCollection services, IConfiguration configuration, Action<DomainSpecificStaticFileOptions> config)
	{
		// Load mapping overrides from configuration
		var overrideOptions = new DomainSpecificStaticFileOptions();
		configuration.GetSection("DomainMappings").Bind(overrideOptions);

		// Initialize the configuration using the action provided by the developer
		var options = new DomainSpecificStaticFileOptions();
		config?.Invoke(options);

		// Override the request mappings if any were provided in config
		// This allows for mapping localhost to a specific domain's resources during development
		if (overrideOptions.RequestMappings is not null && overrideOptions.RequestMappings.Any())
		{
			options.RequestMappings = overrideOptions.RequestMappings;
		}

		// Translate the options provided by the developer into perf-optimised runtime configuration
		var runtimeConfig = DomainSpecificStaticFileOptionsTranslator.Translate(options);

		_ = services.AddSingleton(runtimeConfig);
		_ = services.AddTransient(CreateResourceProvider);
		_ = services.AddTransient<DomainSpecificStaticFileMiddleware>();
		_ = services.AddTransient<DomainSpecificStaticFileRequestHandler>();

		return services;
	}

	private static IResourceProvider CreateResourceProvider(IServiceProvider serviceProvider)
	{
		var configuration = serviceProvider.GetRequiredService<DomainSpecificStaticFileConfiguration>();
		return (IResourceProvider)ActivatorUtilities.CreateInstance(serviceProvider, configuration.ResourceProviderType);
	}
}
