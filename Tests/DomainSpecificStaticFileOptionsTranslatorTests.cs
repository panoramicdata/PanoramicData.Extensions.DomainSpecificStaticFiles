using AwesomeAssertions;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests;

public class DomainSpecificStaticFileOptionsTranslatorTests
{
	/// <summary>
	/// Translates options holding a single request mapping for a single domain, asserting that
	/// exactly one domain with one resource mapping came back, and returns that mapping.
	/// </summary>
	private static ResourceMapping TranslateSingleMapping(
		string relativeUrlPrefix,
		string resourcePathPrefix,
		string domain,
		string resourceSubPath,
		string expectedDomain)
	{
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(
		[
			new RequestMapping
			{
				RelativeUrlPrefix = relativeUrlPrefix,
				ResourcePathPrefix = resourcePathPrefix,
				DomainMappings = [new DomainMapping { Domain = domain, ResourceSubPath = resourceSubPath }]
			}
		]);

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(1);
		result.DomainResourceMappings.Keys.Should().Contain(expectedDomain);

		var domainMapping = result.DomainResourceMappings[expectedDomain];
		domainMapping.Should().HaveCount(1);

		var resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();

		return resourceMapping;
	}

	private static void AssertMapping(
		ResourceMapping resourceMapping,
		string expectedRequestUrlPrefix,
		string expectedResourcePath)
	{
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be(expectedRequestUrlPrefix);
		resourceMapping.ResourcePath.Should().Be(expectedResourcePath);
	}

	/// <summary>
	/// Domains and request URL prefixes are lowercased and normalised to leading/trailing forward
	/// slashes; the resource path prefix and sub path keep the case they were given, because
	/// resource paths are case-sensitive on Linux.
	/// </summary>
	[Theory]
	[InlineData("/style/", "Style", "localhost", "Default", "localhost", "localhost/style/", "Style/Default/")]
	[InlineData("/style/", "Style", "SITE.DOMAIN1.COM", "Default", "site.domain1.com", "site.domain1.com/style/", "Style/Default/")]
	[InlineData("/STYLE/", "Style", "SITE.DOMAIN1.COM", "Default", "site.domain1.com", "site.domain1.com/style/", "Style/Default/")]
	[InlineData("style", "Style", "site.domain1.com", "Default", "site.domain1.com", "site.domain1.com/style/", "Style/Default/")]
	[InlineData("style", "/Style/", "site.domain1.com", "Default", "site.domain1.com", "site.domain1.com/style/", "Style/Default/")]
	[InlineData("style", "STYLE", "site.domain1.com", "Default", "site.domain1.com", "site.domain1.com/style/", "STYLE/Default/")]
	[InlineData("style", "style", "site.domain1.com", "Default", "site.domain1.com", "site.domain1.com/style/", "style/Default/")]
	[InlineData("style", "/Style/", "site.domain1.com", "default", "site.domain1.com", "site.domain1.com/style/", "Style/default/")]
	public void Translate_SingleRequestMappingSingleDomain_NormalisesUrlPrefixAndPreservesResourcePathCase(
		string relativeUrlPrefix,
		string resourcePathPrefix,
		string domain,
		string resourceSubPath,
		string expectedDomain,
		string expectedRequestUrlPrefix,
		string expectedResourcePath)
	{
		var resourceMapping = TranslateSingleMapping(
			relativeUrlPrefix,
			resourcePathPrefix,
			domain,
			resourceSubPath,
			expectedDomain);

		AssertMapping(resourceMapping, expectedRequestUrlPrefix, expectedResourcePath);
	}

	[Fact]
	public void Translate_SingleRequestMappingThreeDomains_CreatesConfigurationWithThreeDomainsAndOneMappingEach()
	{
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(
		[
			new RequestMapping
			{
				RelativeUrlPrefix = "/style/",
				ResourcePathPrefix = "Style",
				DomainMappings =
				[
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" },
					new DomainMapping { Domain = "site.domain2.com", ResourceSubPath = "Alternative" },
					new DomainMapping { Domain = "site.domain3.com", ResourceSubPath = "Default" },
				]
			}
		]);

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(3);

		foreach (var (domain, expectedResourcePath) in new[]
		{
			("site.domain1.com", "Style/Default/"),
			("site.domain2.com", "Style/Alternative/"),
			("site.domain3.com", "Style/Default/"),
		})
		{
			result.DomainResourceMappings.Keys.Should().Contain(domain);

			var domainMapping = result.DomainResourceMappings[domain];
			domainMapping.Should().HaveCount(1);

			AssertMapping(domainMapping[0], $"{domain}/style/", expectedResourcePath);
		}
	}

	[Fact]
	public void Translate_TwoRequestMappingsTwoDomains_CreatesConfigurationWithTwoDomainsAndTwoMappingsEach()
	{
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(
		[
			new RequestMapping
			{
				RelativeUrlPrefix = "/style/",
				ResourcePathPrefix = "Style",
				DomainMappings =
				[
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" },
					new DomainMapping { Domain = "site.domain2.com", ResourceSubPath = "Alternative" },
				]
			},
			new RequestMapping
			{
				RelativeUrlPrefix = "/substance/",
				ResourcePathPrefix = "Substance",
				DomainMappings =
				[
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "SubstanceA" },
					new DomainMapping { Domain = "site.domain2.com", ResourceSubPath = "SubstanceB" },
				]
			}
		]);

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(2);

		foreach (var (domain, expectedStyleResourcePath, expectedSubstanceResourcePath) in new[]
		{
			("site.domain1.com", "Style/Default/", "Substance/SubstanceA/"),
			("site.domain2.com", "Style/Alternative/", "Substance/SubstanceB/"),
		})
		{
			result.DomainResourceMappings.Keys.Should().Contain(domain);

			var domainMapping = result.DomainResourceMappings[domain];
			domainMapping.Should().HaveCount(2);

			AssertMapping(domainMapping[0], $"{domain}/style/", expectedStyleResourcePath);
			AssertMapping(domainMapping[1], $"{domain}/substance/", expectedSubstanceResourcePath);
		}
	}
}
