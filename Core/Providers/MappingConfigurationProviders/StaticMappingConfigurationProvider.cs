using PanoramicData.Extensions.DomainSpecificStaticFiles.Interfaces;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;

/// <summary>
/// Simple mapping configuration provider for serving static config provided in code
/// </summary>
internal class StaticMappingConfigurationProvider : IMappingConfigurationProvider
{
	private readonly DomainSpecificStaticFileConfiguration _configuration;

	public StaticMappingConfigurationProvider(DomainSpecificStaticFileOptions options)
	{
		_configuration = DomainSpecificStaticFileOptionsTranslator.Translate(options);
	}

	void IMappingConfigurationProvider.StartInitialization(IServiceScopeFactory scopeFactory)
	{
		// There is no initialization to be performed for this provider
	}

	public DomainSpecificStaticFileConfiguration GetConfiguration()
		=> _configuration;
}
