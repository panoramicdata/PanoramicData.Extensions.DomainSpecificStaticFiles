using Microsoft.AspNetCore.Http.Extensions;

namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// The context of an interception request
/// </summary>
internal class InterceptionContext : IInterceptionContext
{
	public InterceptionContext(HttpContext context, DomainMappedStaticFileConfiguration configuration)
	{
		Configuration = configuration;
		HttpContext = context;
		RequestPath = GetRequestPath(context);
		LoweredRequestPath = RequestPath.ToLowerInvariant();
		ResourceMapping = GetResourceMapping(LoweredRequestPath, Configuration);
	}

	/// <summary>
	/// The configuration with which the mapping is being performed
	/// </summary>
	public DomainMappedStaticFileConfiguration Configuration { get; }

	/// <summary>
	/// The underlying HttpContext of the request
	/// </summary>
	public HttpContext HttpContext { get; }

	/// <summary>
	/// The request that is being handled
	/// </summary>
	public HttpRequest Request => HttpContext.Request;

	/// <summary>
	/// The response that is being generated
	/// </summary>
	public HttpResponse Response => HttpContext.Response;

	/// <summary>
	/// The original request path used for resource discovery
	/// </summary>
	public string RequestPath { get; }

	/// <summary>
	/// The lowercase request path used for lookups and conversions
	/// </summary>
	public string LoweredRequestPath { get; }

	/// <summary>
	/// The mapping for this request, or null if it cannot be intercepted
	/// </summary>
	public ResourceMapping? ResourceMapping { get; }

	/// <summary>
	/// Whether the request can be intercepted by this subsystem
	/// </summary>
	public bool CanInterceptRequest => ResourceMapping is not null;

	private static string GetRequestPath(HttpContext context)
	{
		var requestUrl = context.Request.GetEncodedUrl();
		var requestUri = new Uri(requestUrl);
		return requestUri.Host + requestUri.AbsolutePath;
	}

	private static ResourceMapping? GetResourceMapping(string loweredRequestPath, DomainMappedStaticFileConfiguration configuration)
		=> configuration.GetResourceMapping(loweredRequestPath);
}
