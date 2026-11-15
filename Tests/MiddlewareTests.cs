using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Fakes;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests;

public class MiddlewareTests
{
	private static DomainSpecificStaticFileMiddleware CreateMiddleware(bool resourceExists)
		=> new(
			new DomainSpecificStaticFileRequestHandler(new FakeResourceProvider(resourceExists), FakeLoggerFactory.CreateDefaultLogger<DomainSpecificStaticFileRequestHandler>()),
			new StaticMappingConfigurationProvider(MappedOptionsFactory.Create()),
			FakeLoggerFactory.CreateDefaultLogger<DomainSpecificStaticFileMiddleware>());

	private static async Task<(int StatusCode, bool NextCalled)> RunAsync(bool resourceExists, string path)
	{
		var context = MappedOptionsFactory.CreateRequest(path);
		var nextCalled = false;

		await CreateMiddleware(resourceExists).InvokeAsync(context, _ =>
		{
			nextCalled = true;
			return Task.CompletedTask;
		});

		return (context.Response.StatusCode, nextCalled);
	}

	[Theory]
	[InlineData(true, "/style/images/favicon.png", StatusCodes.Status200OK)]
	[InlineData(true, "/STYLE/Images/Favicon.png", StatusCodes.Status200OK)]
	[InlineData(false, "/style/images/favicon.png", StatusCodes.Status404NotFound)]
	public async Task InvokeAsync_MatchingRequest_IsInterceptedAndNotPassedOn(bool resourceExists, string path, int expectedStatusCode)
	{
		var (statusCode, nextCalled) = await RunAsync(resourceExists, path);

		statusCode.Should().Be(expectedStatusCode);
		nextCalled.Should().BeFalse();
	}

	[Fact]
	public async Task InvokeAsync_NonMatchingRequest_IsPassedToNextMiddleware()
	{
		var (statusCode, nextCalled) = await RunAsync(true, "/other/images/favicon.png");

		nextCalled.Should().BeTrue();
		statusCode.Should().Be(StatusCodes.Status200OK);
	}

	[Fact]
	public async Task InvokeAsync_NullArguments_Throw()
	{
		var middleware = CreateMiddleware(true);
		var context = MappedOptionsFactory.CreateRequest("/style/a.png");

		var nullContext = () => middleware.InvokeAsync(null!, _ => Task.CompletedTask);
		var nullNext = () => middleware.InvokeAsync(context, null!);

		await nullContext.Should().ThrowAsync<ArgumentNullException>();
		await nullNext.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public void MappingClasses_Constructors_SetProperties()
	{
		var domainMapping = new DomainMapping { Domain = "example.com", ResourceSubPath = "Sub" };
		var requestMapping = new RequestMapping("/a/", "B/");

		domainMapping.Domain.Should().Be("example.com");
		domainMapping.ResourceSubPath.Should().Be("Sub");
		requestMapping.RelativeUrlPrefix.Should().Be("/a/");
		requestMapping.ResourcePathPrefix.Should().Be("B/");
		requestMapping.DomainMappings.Should().BeEmpty();
	}
}