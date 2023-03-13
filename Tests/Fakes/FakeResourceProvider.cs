namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Fakes;

internal class FakeResourceProvider : IResourceProvider
{
	private readonly bool _resourceExists;

	public FakeResourceProvider(bool resourceExists)
	{
		_resourceExists = resourceExists;
	}

	public Task<ResourceInfo> GetResourceInfoAsync(string resourcePath, CancellationToken cancellationToken)
		=> Task.FromResult(new ResourceInfo { Exists = _resourceExists, ResourcePath = resourcePath });

	public Task<ResourceContentAccessor> GetResourceContentAccessorAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken)
		=> Task.FromResult(new ResourceContentAccessor
		{
			MimeType = "image/png",
			ResourceInfo = resourceInfo,
			ResourceProvider = this,
		});

	public Task<byte[]> GetResourceContentsHashAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken)
		=> Task.FromResult(new byte[0]);

	public Task<Stream> GetResourceContentStreamAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken)
		=> Task.FromResult((Stream)new MemoryStream());

	public Task<DateTime> GetResourceLastModifiedUtcAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken)
		=> Task.FromResult(new DateTime(2023, 03, 13, 11, 31, 43));
}
