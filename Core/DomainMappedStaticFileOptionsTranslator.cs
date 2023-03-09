using System.Globalization;

namespace PanoramicData.Extensions.DomainMappedStaticFiles;

internal static class DomainMappedStaticFileOptionsTranslator
{
	/// <summary>
	/// Translate the options that are provided by the developer to the configuration
	/// we need for running efficient request interceptions
	/// </summary>
	/// <param name="options">The options that are to be transformed/translated</param>
	/// <returns>The configuration that results from the options from the developer</returns>
	public static DomainMappedStaticFileConfiguration Translate(DomainMappedStaticFileOptions options)
	{
		var configuration = new DomainMappedStaticFileConfiguration()
		{
			ResourceProviderType = options.ResourceProviderType,
		};

		foreach (var requestMapping in options.RequestMappings)
		{
			foreach (var domainMapping in requestMapping.DomainMappings)
			{
				var requestPath = CombineRequestPathParts(domainMapping.Domain, requestMapping.RelativeUrlPrefix);
				var resourcePath = CombinePathParts(requestMapping.ResourceContainer, domainMapping.ResourceSubContainer);
				configuration.AddResourceMapping(domainMapping.Domain, new ResourceMapping
				{
					RequestUrlPrefix = requestPath,
					ResourcePath = resourcePath,
					CachingPolicy = requestMapping.OverrideCachingPolicy ?? options.DefaultCachingPolicy
				});
			}
		}

		return configuration;
	}

	private static string CombineRequestPathParts(string pathPrefix, string pathSuffix)
	{
		pathPrefix = pathPrefix.ToLower(CultureInfo.InvariantCulture);
		pathSuffix = pathSuffix.ToLower(CultureInfo.InvariantCulture);

		return CombinePathParts(pathPrefix, pathSuffix);
	}

	private static string CombinePathParts(string pathPrefix, string pathSuffix)
	{
		var combinedPath = string.Empty;

		if (pathPrefix.StartsWith("/", StringComparison.Ordinal))
		{
			pathPrefix = pathPrefix[1..];
		}

		combinedPath += pathPrefix;

		if (!combinedPath.EndsWith("/", StringComparison.Ordinal))
		{
			combinedPath += "/";
		}

		if (pathSuffix.StartsWith("/", StringComparison.Ordinal))
		{
			pathSuffix = pathSuffix[1..];
		}

		combinedPath += pathSuffix;

		if (!combinedPath.EndsWith("/", StringComparison.Ordinal))
		{
			combinedPath += "/";
		}

		return combinedPath;
	}
}
