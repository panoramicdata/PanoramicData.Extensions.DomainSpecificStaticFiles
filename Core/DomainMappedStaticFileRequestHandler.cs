namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// Request handler for a domain mapped static resource
/// </summary>
internal class DomainMappedStaticFileRequestHandler
{
	private readonly IResourceProvider _resourceProvider;
	private readonly ILogger _logger;

	public DomainMappedStaticFileRequestHandler(IResourceProvider resourceProvider, ILogger<DomainMappedStaticFileRequestHandler> logger)
	{
		_resourceProvider = resourceProvider;
		_logger = logger;
	}

	/// <summary>
	/// Perform interception of a request
	/// </summary>
	/// <param name="interceptionContext">The context of the interception being performed</param>
	internal async Task InterceptRequestAsync(IInterceptionContext interceptionContext, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(interceptionContext, nameof(interceptionContext));
		ArgumentNullException.ThrowIfNull(interceptionContext.ResourceMapping, nameof(interceptionContext.ResourceMapping));

		_logger.LogDebug("Interception of request by domain mapped static files middleware");

		var resourcePath = ResourcePathCalculator.GetResourcePath(interceptionContext);
		var resourceInfo = await _resourceProvider.GetResourceInfoAsync(resourcePath, cancellationToken);
		if (!resourceInfo.Exists)
		{
			// No matching resource found!
			_logger.LogInformation("Interception of static resource request failed - {Uri} not found", interceptionContext.RequestPath);
			interceptionContext.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
			return;
		}

		using var resourceContentAccessor = await _resourceProvider.GetResourceContentAccessorAsync(resourceInfo, cancellationToken);

		await WriteResponseAsync(interceptionContext, resourceContentAccessor, cancellationToken);
	}

	/// <summary>
	/// Write the response for the requested resource
	/// </summary>
	/// <param name="interceptionContext">The context of the interception request</param>
	/// <param name="resourceContentAccessor">The accessor used to retrieve the contents of the resource</param>
	private static async Task WriteResponseAsync(IInterceptionContext interceptionContext, ResourceContentAccessor resourceContentAccessor, CancellationToken cancellationToken)
	{
		var response = interceptionContext.HttpContext.Response;
		response.StatusCode = StatusCodes.Status200OK;
		response.ContentType = resourceContentAccessor.MimeType;

		WriteResponseHeaders(interceptionContext, resourceContentAccessor);

		using var stream = await resourceContentAccessor.GetResourceContentStreamAsync(cancellationToken);
		int bytesRead;
		var offset = 0;
		var buffer = new byte[4096].AsMemory();
		do
		{
			bytesRead = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
			_ = await response.BodyWriter.WriteAsync(buffer[..bytesRead], cancellationToken).ConfigureAwait(false);
			offset += bytesRead;
		} while (bytesRead > 0);
	}

	/// <summary>
	/// Write the headers for the response associated with a resource
	/// </summary>
	/// <param name="interceptionContext">The context of the interception request</param>
	private static void WriteResponseHeaders(IInterceptionContext interceptionContext, ResourceContentAccessor resourceContentAccessor)
	{
		var cachingPolicy = interceptionContext.ResourceMapping!.CachingPolicy;
		var cachingStrategy = cachingPolicy.CreateCachingStrategy();

		cachingStrategy.WriteCacheHeaders(interceptionContext, resourceContentAccessor);
	}
}
