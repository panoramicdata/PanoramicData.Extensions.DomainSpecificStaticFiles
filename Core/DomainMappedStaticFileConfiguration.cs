using System.Collections.Concurrent;

namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// Configuration used for interception requests within the DomainMappedStaticFileMiddleware
/// </summary>
public class DomainMappedStaticFileConfiguration
{
	private readonly ConcurrentDictionary<string, IList<ResourceMapping>> _domainResourceMappings = new();

	/// <summary>
	/// Get the resource mapping associated with the requested path, or null if no mapping exists
	/// </summary>
	/// <param name="loweredRequestPath">The lowercase path of the request whose mapping we are trying to find</param>
	/// <returns>The mapping that is to be used in interception, or null if no interception is appropriate</returns>
	internal ResourceMapping? GetResourceMapping(string loweredRequestPath)
	{
		// Do very fast lookups of mappings in a case-insensitive manner by using lowercase Url
		var host = GetHost(loweredRequestPath);
		if (!_domainResourceMappings.TryGetValue(host, out var resourceMappings))
		{
			return null;
		}

		var resourceMapping = resourceMappings.FirstOrDefault(m => loweredRequestPath.StartsWith(m.RequestUrlPrefix, StringComparison.Ordinal));
		return resourceMapping;
	}

	/// <summary>
	/// Add a new resource mapping to the configuration
	/// </summary>
	/// <param name="domain">The domain of which this mapping forms a part</param>
	/// <param name="resourceMapping">The mapping to be performed</param>
	internal void AddResourceMapping(string domain, ResourceMapping resourceMapping)
	{
		domain = domain.ToLowerInvariant();
		if (!_domainResourceMappings.ContainsKey(domain))
		{
			_domainResourceMappings.TryAdd(domain, new List<ResourceMapping>());
		}

		_domainResourceMappings[domain].Add(resourceMapping);
	}

	/// <summary>
	/// The dictionary of mappings, exposed to enable unit testing of the translator
	/// </summary>
	internal IDictionary<string, IList<ResourceMapping>> DomainResourceMappings => _domainResourceMappings;

	/// <summary>
	/// The type used to access and serve resources
	/// </summary>
	internal Type ResourceProviderType { get; set; } = typeof(PhysicalFileResourceProvider);

	/// <summary>
	/// Extract the host name from a request path
	/// </summary>
	/// <param name="requestPath">The request path</param>
	/// <returns>The host name, or the full path if the format is not recognised</returns>
	private static string GetHost(string requestPath)
	{
		var firstSeparatorIndex = requestPath.IndexOf('/');
		return firstSeparatorIndex < 0
			? requestPath
			: requestPath[..firstSeparatorIndex];
	}
}