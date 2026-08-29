using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Fakes;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests;

public class DomainSpecificStaticFileRequestHandlerTests
{
	[Fact]
	public async Task InterceptRequestAsync_InterceptionContextIsNull_ThrowsArgumentNullException()
	{
		var handler = new DomainSpecificStaticFileRequestHandler(new FakeResourceProvider(true), FakeLoggerFactory.CreateDefaultLogger<DomainSpecificStaticFileRequestHandler>());
		var action = () => handler.InterceptRequestAsync(null!, CancellationToken.None);

		await action.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task InterceptRequestAsync_InterceptionContextHasNullResourceMapping_ThrowsArgumentNullException()
	{
		var handler = new DomainSpecificStaticFileRequestHandler(new FakeResourceProvider(true), FakeLoggerFactory.CreateDefaultLogger<DomainSpecificStaticFileRequestHandler>());
		var configuration = DomainSpecificStaticFileConfigurationFactory.CreateConfiguration(new List<(string, ResourceMapping)>());
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
		var handler = new DomainSpecificStaticFileRequestHandler(
			new FakeResourceProvider(true),
			FakeLoggerFactory.CreateDefaultLogger<DomainSpecificStaticFileRequestHandler>());
		var context = FakeInterceptionContextBuilder
			.CreateBuilder()
			.Build();
		await handler.InterceptRequestAsync(context, CancellationToken.None);

		context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
	}

	[Fact]
	public async Task InterceptRequestAsync_ProviderIndicatesResourceExists_AddsDefault7DayMaxAgeCacheControlHeader()
	{
		var handler = new DomainSpecificStaticFileRequestHandler(
			new FakeResourceProvider(true),
			FakeLoggerFactory.CreateDefaultLogger<DomainSpecificStaticFileRequestHandler>());
		var context = FakeInterceptionContextBuilder
			.CreateBuilder()
			.Build();
		await handler.InterceptRequestAsync(context, CancellationToken.None);

		context.Response.Headers.CacheControl.Should().Equal((StringValues)"max-age=604800");
	}

	[Fact]
	public async Task InterceptRequestAsync_ProviderIndicatesResourceDoesNotExist_Returns404NotFound()
	{
		var handler = new DomainSpecificStaticFileRequestHandler(
			new FakeResourceProvider(false),
			FakeLoggerFactory.CreateDefaultLogger<DomainSpecificStaticFileRequestHandler>());
		var context = FakeInterceptionContextBuilder
			.CreateBuilder()
			.Build();
		await handler.InterceptRequestAsync(context, CancellationToken.None);

		context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
	}
}
