using FluentAssertions;
using PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests;

public class DomainMappedStaticFileOptionsTranslatorTests
{
	[Fact]
	public void Translate_SingleRequestMappingSingleDomain_CreatesConfigurationWithOneMapping()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="/style/", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "localhost", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainMappedStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(1);
		result.DomainResourceMappings.Keys.Should().Contain("localhost");
		result.DomainResourceMappings["localhost"].Should().HaveCount(1);
	}

	[Fact]
	public void Translate_SingleRequestMappingSingleUppercaseDomain_CreatesConfigurationWithOneMappingWithLowercaseUrlPrefix()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="/style/", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "SITE.DOMAIN1.COM", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainMappedStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(1);
		result.DomainResourceMappings.Keys.Should().Contain("site.domain1.com");
		result.DomainResourceMappings["site.domain1.com"].Should().HaveCount(1);

		var domainMapping = result.DomainResourceMappings["site.domain1.com"];
		domainMapping.Should().NotBeNull();
		domainMapping.Should().HaveCount(1);

		var resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be("site.domain1.com/style/");
		resourceMapping.ResourcePath.Should().Be("Style/Default/");
	}

	[Fact]
	public void Translate_SingleRequestMappingWithUppercaseUrlPrefixAndSingleDomain_CreatesConfigurationWithOneMappingWithLowercaseUrlPrefix()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="/STYLE/", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "SITE.DOMAIN1.COM", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainMappedStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(1);
		result.DomainResourceMappings.Keys.Should().Contain("site.domain1.com");
		result.DomainResourceMappings["site.domain1.com"].Should().HaveCount(1);

		var domainMapping = result.DomainResourceMappings["site.domain1.com"];
		domainMapping.Should().NotBeNull();
		domainMapping.Should().HaveCount(1);

		var resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be("site.domain1.com/style/");
		resourceMapping.ResourcePath.Should().Be("Style/Default/");
	}

	[Fact]
	public void Translate_SingleRequestMappingWithNoForwardSlashesInUrlPrefix_CreatesConfigurationWithOneMappingWithNormalisedUrlPrefix()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="style", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainMappedStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(1);
		result.DomainResourceMappings.Keys.Should().Contain("site.domain1.com");
		result.DomainResourceMappings["site.domain1.com"].Should().HaveCount(1);

		var domainMapping = result.DomainResourceMappings["site.domain1.com"];
		domainMapping.Should().NotBeNull();
		domainMapping.Should().HaveCount(1);

		var resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be("site.domain1.com/style/");
		resourceMapping.ResourcePath.Should().Be("Style/Default/");
	}

	[Fact]
	public void Translate_SingleRequestMappingWithForwardSlashesInResourcePathPrefix_CreatesConfigurationWithOneMappingWithNormalisedResourcePath()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="style", ResourcePathPrefix = "/Style/",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainMappedStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(1);
		result.DomainResourceMappings.Keys.Should().Contain("site.domain1.com");
		result.DomainResourceMappings["site.domain1.com"].Should().HaveCount(1);

		var domainMapping = result.DomainResourceMappings["site.domain1.com"];
		domainMapping.Should().NotBeNull();
		domainMapping.Should().HaveCount(1);

		var resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.ResourcePath.Should().Be("Style/Default/");
	}

	[Fact]
	public void Translate_SingleRequestMappingWithUppercaseResourcePathPrefix_CreatesConfigurationWithOneMappingWithUppercaseResourcePathPrefix()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="style", ResourcePathPrefix = "STYLE",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainMappedStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(1);
		result.DomainResourceMappings.Keys.Should().Contain("site.domain1.com");
		result.DomainResourceMappings["site.domain1.com"].Should().HaveCount(1);

		var domainMapping = result.DomainResourceMappings["site.domain1.com"];
		domainMapping.Should().NotBeNull();
		domainMapping.Should().HaveCount(1);

		var resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.ResourcePath.Should().Be("STYLE/Default/");
	}

	[Fact]
	public void Translate_SingleRequestMappingWithLowercaseResourcePathPrefix_CreatesConfigurationWithOneMappingWithLowercaseResourcePathPrefix()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="style", ResourcePathPrefix = "style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainMappedStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(1);
		result.DomainResourceMappings.Keys.Should().Contain("site.domain1.com");
		result.DomainResourceMappings["site.domain1.com"].Should().HaveCount(1);

		var domainMapping = result.DomainResourceMappings["site.domain1.com"];
		domainMapping.Should().NotBeNull();
		domainMapping.Should().HaveCount(1);

		var resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.ResourcePath.Should().Be("style/Default/");
	}

	[Fact]
	public void Translate_SingleRequestMappingWithLowercaseResourceSubPath_CreatesConfigurationWithOneMappingWithLowercaseResourceSubPath()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="style", ResourcePathPrefix = "/Style/",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "default" }
				}
			}
		});

		var result = DomainMappedStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(1);
		result.DomainResourceMappings.Keys.Should().Contain("site.domain1.com");
		result.DomainResourceMappings["site.domain1.com"].Should().HaveCount(1);

		var domainMapping = result.DomainResourceMappings["site.domain1.com"];
		domainMapping.Should().NotBeNull();
		domainMapping.Should().HaveCount(1);

		var resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.ResourcePath.Should().Be("Style/default/");
	}

	[Fact]
	public void Translate_SingleRequestMappingTwoDomains_CreatesConfigurationWithTwoDomainsAndOneMappingEach()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="/style/", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" },
					new DomainMapping { Domain = "site.domain2.com", ResourceSubPath = "Alternative" },
				}
			}
		});

		var result = DomainMappedStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(2);
		result.DomainResourceMappings.Keys.Should().Contain("site.domain1.com");

		var domainMapping = result.DomainResourceMappings["site.domain1.com"];
		domainMapping.Should().HaveCount(1);
		var resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be("site.domain1.com/style/");
		resourceMapping.ResourcePath.Should().Be("Style/Default/");

		result.DomainResourceMappings.Keys.Should().Contain("site.domain2.com");
		result.DomainResourceMappings["site.domain2.com"].Should().HaveCount(1);

		domainMapping = result.DomainResourceMappings["site.domain2.com"];
		domainMapping.Should().HaveCount(1);
		resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be("site.domain2.com/style/");
		resourceMapping.ResourcePath.Should().Be("Style/Alternative/");
	}
}
