namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// Request handler for a white labelled static resource
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
	internal async Task InterceptRequestAsync(IInterceptionContext interceptionContext)
	{
		ArgumentNullException.ThrowIfNull(interceptionContext, nameof(interceptionContext));
		ArgumentNullException.ThrowIfNull(interceptionContext.RequestMapping, nameof(interceptionContext.RequestMapping));

		_logger.LogDebug("Interception of request by whitelabelling static files middleware");

		var resourcePath = ResourcePathCalculator.GetResourcePath(interceptionContext);
		var resourceInfo = await _resourceProvider.GetResourceInfoAsync(resourcePath);
		if (!resourceInfo.Exists)
		{
			// No matching resource found!
			_logger.LogInformation("Interception of static resource request failed - {Uri} not found", interceptionContext.RequestPath);
			interceptionContext.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
			return;
		}

		using var resourceContentAccessor = await _resourceProvider.GetResourceContentAccessorAsync(resourceInfo);

		await WriteResponseAsync(interceptionContext, resourceContentAccessor);
	}

	/// <summary>
	/// Write the response for the requested resource
	/// </summary>
	/// <param name="interceptionContext">The context of the interception request</param>
	/// <param name="resourceContentAccessor">The accessor used to retrieve the contents of the resource</param>
	private static async Task WriteResponseAsync(IInterceptionContext interceptionContext, ResourceContentAccessor resourceContentAccessor)
	{
		var response = interceptionContext.HttpContext.Response;
		response.StatusCode = StatusCodes.Status200OK;
		response.ContentType = resourceContentAccessor.MimeType;

		WriteResponseHeaders(interceptionContext);

		int bytesRead;
		var buffer = new byte[4096];
		do
		{
			bytesRead = await resourceContentAccessor.ResourceStream.ReadAsync(buffer);
			_ = await response.BodyWriter.WriteAsync(buffer.AsMemory()[..bytesRead]);
		} while (bytesRead > 0);
	}

	/// <summary>
	/// Write the headers for the response associated with a resource
	/// </summary>
	/// <param name="interceptionContext">The context of the interception request</param>
	private static void WriteResponseHeaders(IInterceptionContext interceptionContext)
	{
		var cachingPeriod = interceptionContext.RequestMapping!.OverrideCachingPeriod ?? interceptionContext.Options.DefaultCachingPeriod;

		// Add caching headers
		interceptionContext.HttpContext.Response.Headers.CacheControl = cachingPeriod.TotalSeconds > 0
			? (Microsoft.Extensions.Primitives.StringValues)$"max-age={cachingPeriod.TotalSeconds:F0}"
			: (Microsoft.Extensions.Primitives.StringValues)"no-cache";

		// TODO: Consider adding Last Modified and ETag headers as an alternative optimisation strategy
	}
}
