using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Cryptography;
using System.Text;

namespace PanoramicData.Extensions.DomainMappedStaticFiles;

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
	public async Task<ResourceInfo> GetResourceInfoAsync(string resourcePath)
	{
		var fullResourcePath = GetFullFilePath(resourcePath);
		if (!File.Exists(fullResourcePath))
		{
			return new ResourceInfo
			{
				Exists = false,
				ResourcePath = fullResourcePath,
				Hash = string.Empty
			};
		}

		// File exists, so return the information about it
		return new ResourceInfo
		{
			Exists = true,
			ResourcePath = fullResourcePath,
			Hash = await GetResourceHashAsync(fullResourcePath)
		};
	}

	/// <summary>
	/// Return the contents of a resource
	/// </summary>
	/// <param name="resourceInfo">The information about the resource being requested</param>
	/// <returns>The contents of the resource</returns>
	/// <exception cref="InvalidOperationException">The resource does not exist</exception>
	public Task<ResourceContentAccessor> GetResourceContentAccessorAsync(ResourceInfo resourceInfo)
	{
		if (!resourceInfo.Exists)
		{
			throw new InvalidOperationException("Cannot get the contents of a resource that does not exist!");
		}

		var fullResourcePath = resourceInfo.ResourcePath;
		_ = new FileExtensionContentTypeProvider().TryGetContentType(fullResourcePath, out var mimeType);
		mimeType ??= "application/octet-stream";
		var stream = File.Open(fullResourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);

		return Task.FromResult(new ResourceContentAccessor()
		{
			ResourceStream = stream,
			MimeType = mimeType,
			Hash = resourceInfo.Hash
		});
	}

	private static Task<string> GetResourceHashAsync(string fullFilePath)
	{
		// Create a cryptographic hash of the resource
		// TODO: This should be a hash of the contents!!
		var bytes = Encoding.UTF8.GetBytes(fullFilePath);
		var hashBytes = SHA256.HashData(bytes);
		return Task.FromResult(Convert.ToBase64String(hashBytes));
	}

	private string GetFullFilePath(string resourcePath)
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
