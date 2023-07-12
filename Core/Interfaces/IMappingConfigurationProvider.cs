namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Interfaces;

internal interface IMappingConfigurationProvider
{
	void StartInitialization();

	DomainSpecificStaticFileConfiguration GetConfiguration();
}
