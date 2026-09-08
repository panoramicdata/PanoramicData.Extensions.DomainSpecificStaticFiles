namespace PanoramicData.Extensions.DomainSpecificStaticFiles;

internal static class DomainSpecificStaticFileOptionsTranslator
{
	/// <summary>
	/// Translate the options that are provided by the developer to the configuration
	/// we need for running efficient request interceptions
	/// </summary>
	/// <param name="options">The options that are to be transformed/translated</param>
	/// <returns>The configuration that results from the options from the developer</returns>
	public static DomainSpecificStaticFileConfiguration Translate(DomainSpecificStaticFileOptions options)
	{
		var configuration = new DomainSpecificStaticFileConfiguration
		{
			ResourceProviderType = options.ResourceProviderType,
		};

		foreach (var requestMapping in options.RequestMappings)
		{
			var existingDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (var domainMapping in requestMapping.DomainMappings)
			{
				ValidateDomainMapping(existingDomains, domainMapping);

				var requestPath = CombineRequestPathParts(domainMapping.Domain, requestMapping.RelativeUrlPrefix);
				var resourcePath = CombinePathParts(requestMapping.ResourcePathPrefix, domainMapping.ResourceSubPath);
				configuration.AddResourceMapping(domainMapping.Domain, new ResourceMapping
				{
					RequestUrlPrefix = requestPath,
					ResourcePath = resourcePath,
					CachingPolicy = domainMapping.OverrideCachingPolicy ?? requestMapping.OverrideCachingPolicy ?? options.DefaultCachingPolicy
				});
			}
		}

		return configuration;
	}

	private static void ValidateDomainMapping(HashSet<string> existingDomains, DomainMapping domainMapping)
	{
		if (existingDomains.Contains(domainMapping.Domain))
		{
			throw new ArgumentException($"Duplicate registration found for domain {domainMapping.Domain}!");
		}

		existingDomains.Add(domainMapping.Domain);
	}

	private static string CombineRequestPathParts(string pathPrefix, string pathSuffix)
	{
		pathPrefix = pathPrefix.ToLowerInvariant();
		pathSuffix = pathSuffix.ToLowerInvariant();

		return CombinePathParts(pathPrefix, pathSuffix);
	}

	private static string CombinePathParts(string pathPrefix, string pathSuffix)
	{
		var combinedPath = string.Empty;

		if (pathPrefix.StartsWith('/'))
		{
			pathPrefix = pathPrefix[1..];
		}

		combinedPath += pathPrefix;

		if (!combinedPath.EndsWith('/'))
		{
			combinedPath += "/";
		}

		if (pathSuffix.StartsWith('/'))
		{
			pathSuffix = pathSuffix[1..];
		}

		combinedPath += pathSuffix;

		if (!combinedPath.EndsWith('/'))
		{
			combinedPath += "/";
		}

		return combinedPath;
	}
}
