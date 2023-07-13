using PanoramicData.Extensions.DomainSpecificStaticFiles;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Interfaces;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for the IServiceCollection interface
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Add the types necessary to serve domain-specific static files, 
	/// using static mapping configuration supplied at the time of registration
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

		_ = services.AddSingleton<IMappingConfigurationProvider>(new StaticMappingConfigurationProvider(options));
		RegisterDefaultServices(services);

		return services;
	}

	/// <summary>
	/// Add the types necessary to serve domain-specific static files, 
	/// using configuration loaded via a delegate after system startup
	/// </summary>
	/// <param name="services">The service collection instance on which we are to operate</param>
	/// <param name="action">The configuration action to be performed to create the configuration options once the system is started</param>
	/// <returns>The instance being extended, to support method chaining</returns>
	public static IServiceCollection AddDomainSpecificStaticFiles(this IServiceCollection services, Func<IServiceScopeFactory, Task<DomainSpecificStaticFileOptions>> action)
	{
		_ = services.AddSingleton<IMappingConfigurationProvider>(new DeferredMappingConfigurationProvider(action));
		RegisterDefaultServices(services);

		return services;
	}

	/// <summary>
	/// Add the types necessary to serve domain-specific static files, 
	/// using mapping configuration loaded by the supplied provider
	/// </summary>
	/// <param name="services">The service collection instance on which we are to operate</param>
	/// <param name="mappingConfigurationProvider">The async configuration provider to be used to get the configuration options</param>
	/// <returns>The instance being extended, to support method chaining</returns>
	public static IServiceCollection AddDomainSpecificStaticFiles(this IServiceCollection services, MappingConfigurationProviderBase mappingConfigurationProvider)
	{
		_ = services.AddSingleton<IMappingConfigurationProvider>(mappingConfigurationProvider);
		RegisterDefaultServices(services);

		return services;
	}

	/// <summary>
	/// Add the types necessary to serve domain-specific static files, 
	/// using mapping configuration loaded from an instance of the supplied provider type
	/// </summary>
	/// <typeparam name="TMappingProvider">The type of the mapping configuration provider to be used to load configuration</typeparam>
	/// <param name="services">The service collection instance on which we are to operate</param>
	/// <returns>The instance being extended, to support method chaining</returns>
	public static IServiceCollection AddDomainSpecificStaticFiles<TMappingProvider>(this IServiceCollection services) where TMappingProvider : MappingConfigurationProviderBase
	{
		_ = services.AddSingleton<IMappingConfigurationProvider, TMappingProvider>();
		RegisterDefaultServices(services);

		return services;
	}

	/// <summary>
	/// Register all of the services needed for all scenarios
	/// </summary>
	/// <param name="services">The service collection into which to register the services</param>
	private static void RegisterDefaultServices(IServiceCollection services)
	{
		_ = services.AddTransient(CreateResourceProvider);
		_ = services.AddTransient<DomainSpecificStaticFileMiddleware>();
		_ = services.AddTransient<DomainSpecificStaticFileRequestHandler>();
		_ = services.AddSingleton<MappingConfigurationProviderInitializer>();
	}

	/// <summary>
	/// Create a resource provider on request
	/// </summary>
	/// <param name="serviceProvider">The service provider from which to obtain services</param>
	/// <returns>An instance of the resource provider to be used for accessing static resources</returns>
	private static IResourceProvider CreateResourceProvider(IServiceProvider serviceProvider)
	{
		var configurationProvider = serviceProvider.GetRequiredService<IMappingConfigurationProvider>();
		var configuration = configurationProvider.GetConfiguration();
		return (IResourceProvider)ActivatorUtilities.CreateInstance(serviceProvider, configuration.ResourceProviderType);
	}
}
