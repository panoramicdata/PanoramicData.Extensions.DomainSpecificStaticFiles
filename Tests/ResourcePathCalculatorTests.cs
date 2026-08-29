using AwesomeAssertions;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests;

public class ResourcePathCalculatorTests
{
	[Fact]
	public void GetResourcePath_RequestUrlPrefixEndsWithForwardSlash_CalculatesCorrectPath()
	{
		var configuration = DomainSpecificStaticFileConfigurationFactory.CreateConfiguration(
			new List<(string Domain, ResourceMapping ResourceMapping)>
			{
				new (){
					Domain = "localhost",
					ResourceMapping = new ResourceMapping
						{
							RequestUrlPrefix = "localhost/style/",
							ResourcePath = "Style/Default",
							CachingPolicy = new NoCacheCachingPolicy()
						}
					}
			});
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.WithConfiguration(configuration)
			.Build();
		var result = ResourcePathCalculator.GetResourcePath(interceptionContext);

		result.Should().Be("Style/Default/images/favicon.png");
	}

	[Fact]
	public void GetResourcePath_RequestUrlPrefixDoesNotEndWithForwardSlash_CalculatesCorrectPath()
	{
		var configuration = DomainSpecificStaticFileConfigurationFactory.CreateConfiguration(
			new List<(string Domain, ResourceMapping ResourceMapping)>
			{
				new (){
					Domain = "localhost",
					ResourceMapping = new ResourceMapping
						{
							RequestUrlPrefix = "localhost/style",
							ResourcePath = "Style/Default",
							CachingPolicy = new NoCacheCachingPolicy()
						}
					}
			});
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.WithConfiguration(configuration)
			.Build();
		var result = ResourcePathCalculator.GetResourcePath(interceptionContext);

		result.Should().Be("Style/Default/images/favicon.png");
	}

	[Fact]
	public void GetResourcePath_ResourcePathStartsAndEndsWithForwardSlash_CalculatesCorrectPath()
	{
		var configuration = DomainSpecificStaticFileConfigurationFactory.CreateConfiguration(
			new List<(string Domain, ResourceMapping ResourceMapping)>
			{
				new (){
					Domain = "localhost",
					ResourceMapping = new ResourceMapping
						{
							RequestUrlPrefix = "localhost/style/",
							ResourcePath = "/Style/Default/",
							CachingPolicy = new NoCacheCachingPolicy()
						}
					}
			});
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.WithConfiguration(configuration)
			.Build();
		var result = ResourcePathCalculator.GetResourcePath(interceptionContext);

		result.Should().Be("Style/Default/images/favicon.png");
	}

	[Fact]
	public void GetResourcePath_ResourcePathDoesNotStartWithForwardSlash_CalculatesCorrectPath()
	{
		var configuration = DomainSpecificStaticFileConfigurationFactory.CreateConfiguration(
			new List<(string Domain, ResourceMapping ResourceMapping)>
			{
				new (){
					Domain = "localhost",
					ResourceMapping = new ResourceMapping
						{
							RequestUrlPrefix = "localhost/style/",
							ResourcePath = "Style/Default/",
							CachingPolicy = new NoCacheCachingPolicy()
						}
					}
			});
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.WithConfiguration(configuration)
			.Build();
		var result = ResourcePathCalculator.GetResourcePath(interceptionContext);

		result.Should().Be("Style/Default/images/favicon.png");
	}

	[Fact]
	public void GetResourcePath_ResourcePathEndsWithForwardSlash_CalculatesCorrectPath()
	{
		var configuration = DomainSpecificStaticFileConfigurationFactory.CreateConfiguration(
			new List<(string Domain, ResourceMapping ResourceMapping)>
			{
				new (){
					Domain = "localhost",
					ResourceMapping = new ResourceMapping
						{
							RequestUrlPrefix = "localhost/style/",
							ResourcePath = "Style/Default/",
							CachingPolicy = new NoCacheCachingPolicy()
						}
					}
			});
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.WithConfiguration(configuration)
			.Build();
		var result = ResourcePathCalculator.GetResourcePath(interceptionContext);

		result.Should().Be("Style/Default/images/favicon.png");
	}

	[Fact]
	public void GetResourcePath_ResourcePathDoesNotEndWithForwardSlash_CalculatesCorrectPath()
	{
		var configuration = DomainSpecificStaticFileConfigurationFactory.CreateConfiguration(
			new List<(string Domain, ResourceMapping ResourceMapping)>
			{
				new (){
					Domain = "localhost",
					ResourceMapping = new ResourceMapping
						{
							RequestUrlPrefix = "localhost/style/",
							ResourcePath = "Style/Default",
							CachingPolicy = new NoCacheCachingPolicy()
						}
					}
			});
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.WithConfiguration(configuration)
			.Build();
		var result = ResourcePathCalculator.GetResourcePath(interceptionContext);

		result.Should().Be("Style/Default/images/favicon.png");
	}

	[Fact]
	public void GetResourcePath_WithNoInterceptionContext_ThrowsArgumentNullException()
	{
		var action = () => ResourcePathCalculator.GetResourcePath(null!);

		action.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void GetResourcePath_WithNoMatchingRequestMapping_ThrowsArgumentNullException()
	{
		var configuration = DomainSpecificStaticFileConfigurationFactory.CreateConfiguration(
			new List<(string Domain, ResourceMapping ResourceMapping)>
			{
				new (){
					Domain = "localhost",
					ResourceMapping = new ResourceMapping
						{
							RequestUrlPrefix = "localhost/style/",
							ResourcePath = "/Style/Default/",
							CachingPolicy = new NoCacheCachingPolicy()
						}
					}
			});
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.ForRequestPath("https://localhost/index.html")
			.WithConfiguration(configuration)
			.Build();
		var action = () => ResourcePathCalculator.GetResourcePath(interceptionContext);

		action.Should().Throw<ArgumentNullException>();
	}
}