using FluentAssertions;
using PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests;

public class ResourcePathCalculatorTests
{
	[Fact]
	public void GetResourcePath_ResourcePrefixUrlEndsWithForwardSlash_CalculatesCorrectPath()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(
			new List<RequestMapping>()
			{
				new RequestMapping { RequestUrlPrefix = "localhost/style/", ResourcePath = "Style/Default/" }
			});
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.WithOptions(options)
			.WithRequestMappingAtIndex(0)
			.Build();
		var result = ResourcePathCalculator.GetResourcePath(interceptionContext);

		result.Should().Be("Style/Default/images/favicon.png");
	}

	[Fact]
	public void GetResourcePath_ResourcePrefixUrlDoesNotEndWithForwardSlash_CalculatesCorrectPath()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(
			new List<RequestMapping>()
			{
				new RequestMapping { RequestUrlPrefix = "localhost/style", ResourcePath = "Style/Default/" }
			});
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.WithOptions(options)
			.WithRequestMappingAtIndex(0)
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
	public void GetResourcePath_WithNoRequestMapping_ThrowsArgumentNullException()
	{
		var options = DomainMappedStaticFileOptionsFactory.CreateOptions(
			new List<RequestMapping>()
			{
				new RequestMapping { RequestUrlPrefix = "localhost/style", ResourcePath = "Style/Default/" }
			});
		var interceptionContext = FakeInterceptionContextBuilder.CreateBuilder()
			.WithOptions(options)
			.Build();
		var action = () => ResourcePathCalculator.GetResourcePath(interceptionContext);

		action.Should().Throw<ArgumentNullException>();
	}
}