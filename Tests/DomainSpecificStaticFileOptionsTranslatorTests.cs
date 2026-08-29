using AwesomeAssertions;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests;

public class DomainSpecificStaticFileOptionsTranslatorTests
{
	[Fact]
	public void Translate_SingleRequestMappingSingleDomain_CreatesConfigurationWithOneMapping()
	{
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="/style/", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "localhost", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(1);
		result.DomainResourceMappings.Keys.Should().Contain("localhost");
		result.DomainResourceMappings["localhost"].Should().HaveCount(1);
	}

	[Fact]
	public void Translate_SingleRequestMappingSingleUppercaseDomain_CreatesConfigurationWithOneMappingWithLowercaseUrlPrefix()
	{
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="/style/", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "SITE.DOMAIN1.COM", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

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
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="/STYLE/", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "SITE.DOMAIN1.COM", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

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
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="style", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

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
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="style", ResourcePathPrefix = "/Style/",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

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
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="style", ResourcePathPrefix = "STYLE",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

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
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="style", ResourcePathPrefix = "style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" }
				}
			}
		});

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

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
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="style", ResourcePathPrefix = "/Style/",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "default" }
				}
			}
		});

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

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
	public void Translate_SingleRequestMappingThreeDomains_CreatesConfigurationWithThreeDomainsAndOneMappingEach()
	{
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="/style/", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" },
					new DomainMapping { Domain = "site.domain2.com", ResourceSubPath = "Alternative" },
					new DomainMapping { Domain = "site.domain3.com", ResourceSubPath = "Default" },
				}
			}
		});

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(3);
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

		result.DomainResourceMappings.Keys.Should().Contain("site.domain3.com");
		domainMapping = result.DomainResourceMappings["site.domain3.com"];
		domainMapping.Should().HaveCount(1);
		resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be("site.domain3.com/style/");
		resourceMapping.ResourcePath.Should().Be("Style/Default/");
	}

	[Fact]
	public void Translate_TwoRequestMappingsTwoDomains_CreatesConfigurationWithTwoDomainsAndTwoMappingsEach()
	{
		var options = DomainSpecificStaticFileOptionsFactory.CreateOptions(new List<RequestMapping>
		{
			new RequestMapping{ RelativeUrlPrefix="/style/", ResourcePathPrefix = "Style",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "Default" },
					new DomainMapping { Domain = "site.domain2.com", ResourceSubPath = "Alternative" },
				}
			},
			new RequestMapping{ RelativeUrlPrefix="/substance/", ResourcePathPrefix = "Substance",
				DomainMappings = new List<DomainMapping>
				{
					new DomainMapping { Domain = "site.domain1.com", ResourceSubPath = "SubstanceA" },
					new DomainMapping { Domain = "site.domain2.com", ResourceSubPath = "SubstanceB" },
				}
			}
		});

		var result = DomainSpecificStaticFileOptionsTranslator.Translate(options);

		result.Should().NotBeNull();
		result.DomainResourceMappings.Should().HaveCount(2);

		result.DomainResourceMappings.Keys.Should().Contain("site.domain1.com");
		var domainMapping = result.DomainResourceMappings["site.domain1.com"];
		domainMapping.Should().HaveCount(2);

		var resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be("site.domain1.com/style/");
		resourceMapping.ResourcePath.Should().Be("Style/Default/");

		resourceMapping = domainMapping[1];
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be("site.domain1.com/substance/");
		resourceMapping.ResourcePath.Should().Be("Substance/SubstanceA/");

		result.DomainResourceMappings.Keys.Should().Contain("site.domain2.com");
		domainMapping = result.DomainResourceMappings["site.domain2.com"];
		domainMapping.Should().HaveCount(2);

		resourceMapping = domainMapping[0];
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be("site.domain2.com/style/");
		resourceMapping.ResourcePath.Should().Be("Style/Alternative/");

		resourceMapping = domainMapping[1];
		resourceMapping.Should().NotBeNull();
		resourceMapping.RequestUrlPrefix.Should().Be("site.domain2.com/substance/");
		resourceMapping.ResourcePath.Should().Be("Substance/SubstanceB/");
	}
}
