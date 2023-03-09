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
		ResourceMapping = GetResourceMapping(RequestPath, Configuration);
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
	/// The request path used for lookups and conversions
	/// </summary>
	public string RequestPath { get; }

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

	private static ResourceMapping? GetResourceMapping(string requestPath, DomainMappedStaticFileConfiguration configuration)
		=> configuration.GetResourceMapping(requestPath);
}
