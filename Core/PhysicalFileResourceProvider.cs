using Microsoft.AspNetCore.StaticFiles;
using System.Security.Cryptography;
using System.Text;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles;

/// <summary>
/// A resource provider that is used to interact with physical files on disk
/// </summary>
internal class PhysicalFileResourceProvider : IResourceProvider
{
	private readonly IWebHostEnvironment _environment;
	private readonly ILogger _logger;

	public PhysicalFileResourceProvider(IWebHostEnvironment environment, ILogger<PhysicalFileResourceProvider> logger)
	{
		_environment = environment;
		_logger = logger;
	}

	/// <summary>
	/// Return information about a physical file resource
	/// </summary>
	/// <param name="resourcePath">The relative path to the resource</param>
	/// <returns>Information about the desired resource, including whether it exists</returns>
	public Task<ResourceInfo> GetResourceInfoAsync(string resourcePath, CancellationToken cancellationToken)
	{
		var fullResourcePath = GetFullFilePath(resourcePath);
		if (!File.Exists(fullResourcePath))
		{
			_logger.LogInformation("Physical file provider: no file found at path '{ResourcePath}'", fullResourcePath);
			return Task.FromResult(new ResourceInfo
			{
				Exists = false,
				ResourcePath = fullResourcePath
			});
		}

		// File exists, so return the information about it
		return Task.FromResult(new ResourceInfo
		{
			Exists = true,
			ResourcePath = fullResourcePath
		});
	}

	/// <summary>
	/// Return an accessor that can be used to access the contents of a resource
	/// </summary>
	/// <param name="resourceInfo">The information about the resource being requested</param>
	/// <returns>The contents of the resource</returns>
	/// <exception cref="InvalidOperationException">The resource does not exist</exception>
	public Task<ResourceContentAccessor> GetResourceContentAccessorAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken)
	{
		if (!resourceInfo.Exists)
		{
			throw new InvalidOperationException("Cannot get the contents of a resource that does not exist!");
		}

		_ = new FileExtensionContentTypeProvider().TryGetContentType(resourceInfo.ResourcePath, out var mimeType);
		mimeType ??= "application/octet-stream";

		return Task.FromResult(new ResourceContentAccessor()
		{
			ResourceInfo = resourceInfo,
			ResourceProvider = this,
			MimeType = mimeType
		});
	}

	/// <summary>
	/// Return a stream that provides access to the contents of a resource
	/// </summary>
	/// <param name="resourceInfo">The information about the resource being requested</param>
	/// <returns>The contents of the resource</returns>
	/// <exception cref="InvalidOperationException">The resource does not exist</exception>
	public Task<Stream> GetResourceContentStreamAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken)
		=> !resourceInfo.Exists
			? throw new InvalidOperationException("Cannot get the contents of a resource that does not exist!")
			: Task.FromResult((Stream)File.Open(resourceInfo.ResourcePath, FileMode.Open, FileAccess.Read, FileShare.Read));

	/// <inheritdoc/>
	public Task<byte[]> GetResourceContentsHashAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken)
	{
		// Create a cryptographic hash of the resource
		// TODO: This should be a hash of the contents!!
		var bytes = Encoding.UTF8.GetBytes(resourceInfo.ResourcePath);
		var hashBytes = SHA256.HashData(bytes);
		return Task.FromResult(hashBytes);
	}

	/// <inheritdoc/>
	public Task<DateTime> GetResourceLastModifiedUtcAsync(ResourceInfo resourceInfo, CancellationToken cancellationToken)
	{
		var fileInfo = new FileInfo(resourceInfo.ResourcePath);
		return Task.FromResult(fileInfo.LastWriteTimeUtc);
	}

	private string GetFullFilePath(string resourcePath)
		=> _environment.ContentRootPath.Contains('\\')
			? GetFullWindowsFilePath(resourcePath)
			: GetFullLinuxFilePath(resourcePath);

	private string GetFullLinuxFilePath(string resourcePath)
	{
		if (resourcePath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			resourcePath = resourcePath[1..];
		}

		var contentRootPath = _environment.ContentRootPath;
		if (!contentRootPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			contentRootPath += "/";
		}

		var fullResourcePath = contentRootPath + resourcePath;
		return fullResourcePath;
	}

	private string GetFullWindowsFilePath(string resourcePath)
	{
		var resourceSubPath = resourcePath.Replace("/", @"\");
		if (resourceSubPath.StartsWith(@"\", StringComparison.OrdinalIgnoreCase))
		{
			resourceSubPath = resourceSubPath[1..];
		}

		var fullResourcePath = Path.Combine(_environment.ContentRootPath, resourceSubPath);
		return fullResourcePath;
	}
}
