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
		var hash = SHA256.HashData(Encoding.UTF8.GetBytes(uri.Host.ToLowerInvariant()));
		return Match(_hashedMappingsDictionary, hash, uri);
	}

	private RequestMapping? LookupFromCaseSensitiveDictionary(string requestUrl)
	{
		var uri = new Uri(requestUrl.ToLowerInvariant());
		return Match(_caseSensitiveMappingsDictionary, uri.Host, uri);
	}

	private RequestMapping? LookupFromCaseInsensitiveDictionary(string requestUrl)
	{
		var uri = new Uri(requestUrl);
		return Match(_caseInsensitiveMappingsDictionary, uri.Host, uri);
	}

	private static RequestMapping? Match<TKey>(ConcurrentDictionary<TKey, List<RequestMapping>> dictionary, TKey key, Uri uri) where TKey : notnull
	{
		var lookupPath = uri.Host + "/" + uri.AbsoluteUri;
		return dictionary.TryGetValue(key, out var mappings)
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

	private static IEnumerable<(string Host, int Index)> Hosts()
		=> from environment in new[] { "production", "test" }
			from index in Enumerable.Range(1, 25)
			select ($"{environment}.domain{index}.com", index);

	private static ConcurrentDictionary<TKey, List<RequestMapping>> BuildDictionary<TKey>(Func<string, TKey> toKey, IEqualityComparer<TKey>? comparer = null) where TKey : notnull
	{
		var mappings = new ConcurrentDictionary<TKey, List<RequestMapping>>(comparer);

		foreach (var (host, index) in Hosts())
		{
			mappings.TryAdd(toKey(host), [new() { RequestPathPrefix = "/style/", ResourcePath = $"Style/Domain{index}/" }]);
		}

		return mappings;
	}

	private static ConcurrentDictionary<byte[], List<RequestMapping>> BuildHashedMappingsDictionary()
		=> BuildDictionary(host => SHA256.HashData(Encoding.UTF8.GetBytes(host)));

	private static ConcurrentDictionary<string, List<RequestMapping>> BuildCaseInsensitiveMappingsDictionary()
		=> BuildDictionary(host => host, StringComparer.InvariantCultureIgnoreCase);

	private static ConcurrentDictionary<string, List<RequestMapping>> BuildCaseSensitiveMappingsDictionary()
		=> BuildDictionary(host => host);

	private static List<RequestMapping> BuildMappingsList()
		=> Hosts().Select(h => new RequestMapping { RequestPathPrefix = $"{h.Host}/style/", ResourcePath = $"/Style/Domain{h.Index}" }).ToList();

	#endregion
}