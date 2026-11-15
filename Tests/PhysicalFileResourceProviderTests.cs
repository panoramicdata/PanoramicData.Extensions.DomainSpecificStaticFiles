using AwesomeAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.ResourceProviders;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests;

public sealed class PhysicalFileResourceProviderTests : IDisposable
{
	private readonly string _root = Path.Combine(Path.GetTempPath(), "dssf-" + Guid.NewGuid().ToString("N"));

	public PhysicalFileResourceProviderTests()
	{
		_ = Directory.CreateDirectory(Path.Combine(_root, "Style"));
		File.WriteAllText(Path.Combine(_root, "Style", "site.css"), "body{}");
		File.WriteAllText(Path.Combine(_root, "Style", "data.unknownext"), "x");
	}

	public void Dispose() => Directory.Delete(_root, true);

	private PhysicalFileResourceProvider CreateProvider()
		=> new(new TestEnvironment(_root), FakeLoggerFactory.CreateDefaultLogger<PhysicalFileResourceProvider>());

	[Theory]
	[InlineData("Style/site.css", true)]
	[InlineData("/Style/site.css", true)]
	[InlineData("Style/missing.css", false)]
	public async Task GetResourceInfoAsync_ReportsExistence(string resourcePath, bool expected)
	{
		var info = await CreateProvider().GetResourceInfoAsync(resourcePath, CancellationToken.None);

		info.Exists.Should().Be(expected);
		info.ResourcePath.Should().Contain("Style");
	}

	[Theory]
	[InlineData("Style/site.css", "text/css")]
	[InlineData("Style/data.unknownext", "application/octet-stream")]
	public async Task GetResourceContentAccessorAsync_ResolvesMimeType(string resourcePath, string expectedMimeType)
	{
		var provider = CreateProvider();
		var info = await provider.GetResourceInfoAsync(resourcePath, CancellationToken.None);

		using var accessor = await provider.GetResourceContentAccessorAsync(info, CancellationToken.None);

		accessor.MimeType.Should().Be(expectedMimeType);
	}

	[Fact]
	public async Task GetResourceContentStreamAsync_ExistingFile_ReturnsContents()
	{
		var provider = CreateProvider();
		var info = await provider.GetResourceInfoAsync("Style/site.css", CancellationToken.None);

		await using var stream = await provider.GetResourceContentStreamAsync(info, CancellationToken.None);
		using var reader = new StreamReader(stream);

		(await reader.ReadToEndAsync(TestContext.Current.CancellationToken)).Should().Be("body{}");
	}

	[Fact]
	public async Task MissingResource_AccessorAndStream_Throw()
	{
		var provider = CreateProvider();
		var info = await provider.GetResourceInfoAsync("Style/missing.css", CancellationToken.None);

		var accessor = () => provider.GetResourceContentAccessorAsync(info, CancellationToken.None);
		var stream = () => provider.GetResourceContentStreamAsync(info, CancellationToken.None);

		await accessor.Should().ThrowAsync<InvalidOperationException>();
		await stream.Should().ThrowAsync<InvalidOperationException>();
	}

	[Fact]
	public async Task HashAndLastModified_ForExistingFile_AreProvided()
	{
		var provider = CreateProvider();
		var info = await provider.GetResourceInfoAsync("Style/site.css", CancellationToken.None);

		var hash = await provider.GetResourceContentsHashAsync(info, CancellationToken.None);
		var lastModified = await provider.GetResourceLastModifiedUtcAsync(info, CancellationToken.None);

		hash.Should().HaveCount(32);
		lastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(5));
	}

	private sealed class TestEnvironment(string contentRootPath) : IWebHostEnvironment
	{
		public string WebRootPath { get; set; } = contentRootPath;

		public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();

		public string ApplicationName { get; set; } = "Tests";

		public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();

		public string ContentRootPath { get; set; } = contentRootPath;

		public string EnvironmentName { get; set; } = "Test";
	}
}