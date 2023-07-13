namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Interfaces;

internal interface IMappingConfigurationProvider
{
	void StartInitialization(IServiceScopeFactory scopeFactory);

	DomainSpecificStaticFileConfiguration GetConfiguration();
}
