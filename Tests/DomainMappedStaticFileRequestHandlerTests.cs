using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Fakes;
using PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests;

public class DomainMappedStaticFileRequestHandlerTests
{
	[Fact]
	public async Task InterceptRequestAsync_InterceptionContextIsNull_ThrowsArgumentNullException()
	{
		var handler = new DomainMappedStaticFileRequestHandler(new FakeResourceProvider(true), FakeLoggerFactory.CreateDefaultLogger<DomainMappedStaticFileRequestHandler>());
		var action = () => handler.InterceptRequestAsync(null!, CancellationToken.None);

		await action.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task InterceptRequestAsync_InterceptionContextHasNullResourceMapping_ThrowsArgumentNullException()
	{
		var handler = new DomainMappedStaticFileRequestHandler(new FakeResourceProvider(true), FakeLoggerFactory.CreateDefaultLogger<DomainMappedStaticFileRequestHandler>());
		var configuration = DomainMappedStaticFileConfigurationFactory.CreateConfiguration(new List<(string, ResourceMapping)>());
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
		var handler = new DomainMappedStaticFileRequestHandler(
			new FakeResourceProvider(true),
			FakeLoggerFactory.CreateDefaultLogger<DomainMappedStaticFileRequestHandler>());
		var context = FakeInterceptionContextBuilder
			.CreateBuilder()
			.Build();
		await handler.InterceptRequestAsync(context, CancellationToken.None);

		context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
	}

	[Fact]
	public async Task InterceptRequestAsync_ProviderIndicatesResourceExists_AddsDefault7DayMaxAgeCacheControlHeader()
	{
		var handler = new DomainMappedStaticFileRequestHandler(
			new FakeResourceProvider(true),
			FakeLoggerFactory.CreateDefaultLogger<DomainMappedStaticFileRequestHandler>());
		var context = FakeInterceptionContextBuilder
			.CreateBuilder()
			.Build();
		await handler.InterceptRequestAsync(context, CancellationToken.None);

		context.Response.Headers.CacheControl.Should().Equal((StringValues)"max-age=604800");
	}

	[Fact]
	public async Task InterceptRequestAsync_ProviderIndicatesResourceDoesNotExist_Returns404NotFound()
	{
		var handler = new DomainMappedStaticFileRequestHandler(
			new FakeResourceProvider(false),
			FakeLoggerFactory.CreateDefaultLogger<DomainMappedStaticFileRequestHandler>());
		var context = FakeInterceptionContextBuilder
			.CreateBuilder()
			.Build();
		await handler.InterceptRequestAsync(context, CancellationToken.None);

		context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
	}
}
