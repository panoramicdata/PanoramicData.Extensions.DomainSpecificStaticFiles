using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Benchmarks;

[MemoryDiagnoser]
public class LookupBenchmarks
{
	private readonly ConcurrentDictionary<byte[], List<RequestMapping>> _hashedMappingsDictionary = BuildHashedMappingsDictionary();
	private readonly ConcurrentDictionary<string, List<RequestMapping>> _caseInsensitiveMappingsDictionary = BuildCaseInsensitiveMappingsDictionary();
	private readonly ConcurrentDictionary<string, List<RequestMapping>> _caseSensitiveMappingsDictionary = BuildCaseSensitiveMappingsDictionary();
	private readonly List<RequestMapping> _mappingsList = BuildMappingsList();

	[ParamsSource(nameof(RequestUrls))]
	public string RequestUrl { get; set; } = string.Empty;

	public static IEnumerable<string> RequestUrls => new List<string>{
		"https://production.domain1.com/style/images/favicon.png",
		"https://production.domain4.com/style/images/favicon.png",
		"https://production.domain8.com/style/images/favicon.png",
		"https://production.domain10.com/style/images/favicon.png",
		"https://production.domain15.com/style/images/favicon.png",
		"https://production.domain20.com/style/images/favicon.png",
		"https://production.domain22.com/style/images/favicon.png",
		"https://production.domain25.com/style/images/favicon.png",
		"https://test.domain8.com/style/images/favicon.png",
	};

	[Benchmark]
	public void HashedDictionaryLookup() => _ = LookupFromHashedDictionary(RequestUrl);

	[Benchmark]
	public void CaseInsensitiveDictionaryLookup() => _ = LookupFromCaseInsensitiveDictionary(RequestUrl);

	[Benchmark]
	public void LoweredDictionaryLookup() => _ = LookupFromCaseSensitiveDictionary(RequestUrl);

	[Benchmark]
	public void ListLookup() => _ = LookupFromList(RequestUrl);

	private RequestMapping? LookupFromHashedDictionary(string requestUrl)
	{
		var uri = new Uri(requestUrl);
		var hash = SHA256.HashData(Encoding.UTF8.GetBytes(uri.Host.ToLower()));
		var lookupPath = uri.Host + "/" + uri.AbsoluteUri;
		return _hashedMappingsDictionary.TryGetValue(hash, out var mappings)
			? mappings.FirstOrDefault(m => lookupPath.StartsWith(uri.Host + m.RequestPathPrefix))
			: null;
	}

	private RequestMapping? LookupFromCaseSensitiveDictionary(string requestUrl)
	{
		var uri = new Uri(requestUrl.ToLower());
		var lookupPath = uri.Host + "/" + uri.AbsoluteUri;
		return _caseSensitiveMappingsDictionary.TryGetValue(uri.Host, out var mappings)
			? mappings.FirstOrDefault(m => lookupPath.StartsWith(uri.Host + m.RequestPathPrefix))
			: null;
	}

	private RequestMapping? LookupFromCaseInsensitiveDictionary(string requestUrl)
	{
		var uri = new Uri(requestUrl);
		var lookupPath = uri.Host + "/" + uri.AbsoluteUri;
		return _caseInsensitiveMappingsDictionary.TryGetValue(uri.Host, out var mappings)
			? mappings.FirstOrDefault(m => lookupPath.StartsWith(uri.Host + m.RequestPathPrefix))
			: null;
	}

	public RequestMapping? LookupFromList(string requestUrl)
	{
		var uri = new Uri(requestUrl);
		var lookupPath = uri.Host + "/" + uri.AbsoluteUri;
		return _mappingsList.FirstOrDefault(m => lookupPath.StartsWith(m.RequestPathPrefix, StringComparison.InvariantCultureIgnoreCase));
	}

	#region Initialisation

	private static ConcurrentDictionary<byte[], List<RequestMapping>> BuildHashedMappingsDictionary()
	{
		var mappings = new ConcurrentDictionary<byte[], List<RequestMapping>>();

		foreach (var environment in new List<string>() { "production", "test" })
		{
			foreach (var index in Enumerable.Range(1, 25))
			{
				var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{environment}.domain{index}.com"));
				mappings.TryAdd(hash, new List<RequestMapping>()
				{
					new RequestMapping()
					{
						RequestPathPrefix = "/style/",
						ResourcePath = $"Style/Domain{index}/"
					}
				});
			}
		}

		return mappings;
	}

	private static ConcurrentDictionary<string, List<RequestMapping>> BuildCaseInsensitiveMappingsDictionary()
	{
		var mappings = new ConcurrentDictionary<string, List<RequestMapping>>(StringComparer.InvariantCultureIgnoreCase);

		foreach (var environment in new List<string>() { "production", "test" })
		{
			foreach (var index in Enumerable.Range(1, 25))
			{
				mappings.TryAdd($"{environment}.domain{index}.com", new List<RequestMapping>()
				{
					new RequestMapping()
					{
						RequestPathPrefix = "/style/",
						ResourcePath = $"Style/Domain{index}/"
					}
				});
			}
		}

		return mappings;
	}

	private static ConcurrentDictionary<string, List<RequestMapping>> BuildCaseSensitiveMappingsDictionary()
	{
		var mappings = new ConcurrentDictionary<string, List<RequestMapping>>();

		foreach (var environment in new List<string>() { "production", "test" })
		{
			foreach (var index in Enumerable.Range(1, 25))
			{
				mappings.TryAdd($"{environment}.domain{index}.com", new List<RequestMapping>()
				{
					new RequestMapping()
					{
						RequestPathPrefix = "/style/",
						ResourcePath = $"Style/Domain{index}/"
					}
				});
			}
		}

		return mappings;
	}

	private static List<RequestMapping> BuildMappingsList()
	{
		var mappings = new List<RequestMapping>();

		foreach (var environment in new List<string>() { "production", "test" })
		{
			foreach (var index in Enumerable.Range(1, 25))
			{
				mappings.Add(new() { RequestPathPrefix = $"{environment}.domain{index}.com/style/", ResourcePath = $"/Style/Domain{index}" });
			}
		}

		return mappings;
	}

	#endregion
}
