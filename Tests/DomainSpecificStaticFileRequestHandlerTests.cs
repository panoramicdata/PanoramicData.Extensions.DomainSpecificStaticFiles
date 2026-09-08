using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Fakes;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests;

public class DomainSpecificStaticFileRequestHandlerTests
{
	private static DomainSpecificStaticFileRequestHandler CreateHandler(bool resourceExists)
		=> new(
			new FakeResourceProvider(resourceExists),
			FakeLoggerFactory.CreateDefaultLogger<DomainSpecificStaticFileRequestHandler>());

	/// <summary>
	/// Runs a request against a handler whose provider reports the resource as existing or not,
	/// and returns the context so the response can be inspected.
	/// </summary>
	private static async Task<IInterceptionContext> InterceptDefaultRequestAsync(bool resourceExists)
	{
		var handler = CreateHandler(resourceExists);
		var context = FakeInterceptionContextBuilder
			.CreateBuilder()
			.Build();

		await handler.InterceptRequestAsync(context, CancellationToken.None);

		return context;
	}

	[Fact]
	public async Task InterceptRequestAsync_InterceptionContextIsNull_ThrowsArgumentNullException()
	{
		var handler = CreateHandler(true);

		var action = () => handler.InterceptRequestAsync(null!, CancellationToken.None);

		await action.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task InterceptRequestAsync_InterceptionContextHasNullResourceMapping_ThrowsArgumentNullException()
	{
		var handler = CreateHandler(true);
		var configuration = DomainSpecificStaticFileConfigurationFactory.CreateConfiguration([]);
		var context = FakeInterceptionContextBuilder
			.CreateBuilder()
			.ForRequestPath("localhost/notfound/nosuchimage.png")
			.WithConfiguration(configuration)
			.Build();

		var action = () => handler.InterceptRequestAsync(context, CancellationToken.None);

		await action.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task InterceptRequestAsync_ProviderIndicatesResourceExists_Returns200OK()
	{
		var context = await InterceptDefaultRequestAsync(true);

		context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
	}

	[Fact]
	public async Task InterceptRequestAsync_ProviderIndicatesResourceExists_AddsDefault7DayMaxAgeCacheControlHeader()
	{
		var context = await InterceptDefaultRequestAsync(true);

		context.Response.Headers.CacheControl.Should().Equal((StringValues)"max-age=604800");
	}

	[Fact]
	public async Task InterceptRequestAsync_ProviderIndicatesResourceDoesNotExist_Returns404NotFound()
	{
		var context = await InterceptDefaultRequestAsync(false);

		context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
	}
}
