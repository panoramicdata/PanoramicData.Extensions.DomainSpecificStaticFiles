using AwesomeAssertions;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Caching;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests;

public class ResourcePathCalculatorTests
{
	private const string ExpectedResourcePath = "Style/Default/images/favicon.png";

	/// <summary>
	/// Leading and trailing forward slashes on either the request URL prefix or the resource path
	/// are normalised away, so every combination resolves to the same resource path.
	/// </summary>
	[Theory]
	[InlineData("localhost/style/", "Style/Default")]  // Prefix ends with a forward slash
	[InlineData("localhost/style", "Style/Default")]   // Prefix does not end with a forward slash
	[InlineData("localhost/style/", "/Style/Default/")] // Resource path starts and ends with a forward slash
	[InlineData("localhost/style/", "Style/Default/")]  // Resource path ends with a forward slash only
	public void GetResourcePath_WithVariedForwardSlashes_CalculatesCorrectPath(string requestUrlPrefix, string resourcePath)
	{
		var configuration = DomainSpecificStaticFileConfigurationFactory
			.CreateSingleMappingConfiguration(requestUrlPrefix, resourcePath, new NoCacheCachingPolicy());
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.WithConfiguration(configuration)
			.Build();

		var result = ResourcePathCalculator.GetResourcePath(interceptionContext);

		result.Should().Be(ExpectedResourcePath);
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
		var configuration = DomainSpecificStaticFileConfigurationFactory
			.CreateSingleMappingConfiguration("localhost/style/", "/Style/Default/", new NoCacheCachingPolicy());
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.ForRequestPath("https://localhost/index.html")
			.WithConfiguration(configuration)
			.Build();

		var action = () => ResourcePathCalculator.GetResourcePath(interceptionContext);

		action.Should().Throw<ArgumentNullException>();
	}
}
