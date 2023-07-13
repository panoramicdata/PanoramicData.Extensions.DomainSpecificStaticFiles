using PanoramicData.Extensions.DomainSpecificStaticFiles.Interfaces;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;

internal class MappingConfigurationProviderInitializer
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IMappingConfigurationProvider _mappingConfigurationProvider;

	public MappingConfigurationProviderInitializer(IServiceScopeFactory scopeFactory, IMappingConfigurationProvider mappingConfigurationProvider)
	{
		_scopeFactory = scopeFactory;
		_mappingConfigurationProvider = mappingConfigurationProvider;
	}

	internal void StartInitialization()
		=> _mappingConfigurationProvider.StartInitialization(_scopeFactory);
}
