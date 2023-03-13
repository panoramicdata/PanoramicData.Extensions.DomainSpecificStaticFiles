using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;
internal static class DomainSpecificStaticFileConfigurationFactory
{
	public static DomainSpecificStaticFileConfiguration CreateDefaultConfiguration()
	{
		var configuration = new DomainSpecificStaticFileConfiguration();

		configuration.AddResourceMapping("localhost", new()
		{
			RequestUrlPrefix = "localhost",
			ResourcePath = "Style/Default",
			CachingPolicy = new MaxAgeCachingPolicy()
		});

		return configuration;
	}

	public static DomainSpecificStaticFileConfiguration CreateConfiguration(IEnumerable<(string Domain, ResourceMapping ResourceMapping)> mappings)
	{
		var configuration = new DomainSpecificStaticFileConfiguration();

		foreach (var mapping in mappings)
		{
			configuration.AddResourceMapping(mapping.Domain, mapping.ResourceMapping);
		}

		return configuration;
	}
}
