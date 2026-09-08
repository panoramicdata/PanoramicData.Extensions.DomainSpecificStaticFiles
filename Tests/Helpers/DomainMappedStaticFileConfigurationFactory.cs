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

	/// <summary>
	/// Creates a configuration containing a single localhost mapping. Most tests vary only the
	/// prefix, the path or the policy, so the rest of the mapping is built here.
	/// </summary>
	public static DomainSpecificStaticFileConfiguration CreateSingleMappingConfiguration(
		string requestUrlPrefix,
		string resourcePath,
		ICachingPolicy? cachingPolicy = null)
		=> CreateConfiguration(
		[
			("localhost", new ResourceMapping
			{
				RequestUrlPrefix = requestUrlPrefix,
				ResourcePath = resourcePath,
				CachingPolicy = cachingPolicy ?? new NoCacheCachingPolicy()
			})
		]);

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
