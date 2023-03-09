using PanoramicData.Extensions.DomainMappedStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;
internal static class DomainMappedStaticFileConfigurationFactory
{
	public static DomainMappedStaticFileConfiguration CreateDefaultConfiguration()
	{
		var configuration = new DomainMappedStaticFileConfiguration();

		configuration.AddResourceMapping("localhost", new()
		{
			RequestUrlPrefix = "localhost",
			ResourcePath = "Style/Default",
			CachingPolicy = new NoCacheCachingPolicy()
		});

		return configuration;
	}

	public static DomainMappedStaticFileConfiguration CreateConfiguration(IEnumerable<(string Domain, ResourceMapping ResourceMapping)> mappings)
	{
		var configuration = new DomainMappedStaticFileConfiguration();

		foreach (var mapping in mappings)
		{
			configuration.AddResourceMapping(mapping.Domain, mapping.ResourceMapping);
		}

		return configuration;
	}
}
