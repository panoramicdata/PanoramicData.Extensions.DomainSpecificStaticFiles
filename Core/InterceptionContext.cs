using Microsoft.AspNetCore.Http.Extensions;

namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// The context of an interception request
/// </summary>
internal class InterceptionContext : IInterceptionContext
{
	public InterceptionContext(HttpContext context, DomainMappedStaticFileOptions options)
	{
		Options = options;
		HttpContext = context;
		RequestPath = GetRequestPath(context);
		RequestMapping = GetResourceMapping(RequestPath, options);
	}

	/// <summary>
	/// The options with which the mapping is being performed
	/// </summary>
	public DomainMappedStaticFileOptions Options { get; }

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
	public RequestMapping? RequestMapping { get; }

	/// <summary>
	/// Whether the request can be intercepted by this subsystem
	/// </summary>
	public bool CanInterceptRequest => RequestMapping is not null;

	private static string GetRequestPath(HttpContext context)
	{
		var requestUrl = context.Request.GetEncodedUrl();
		var requestUri = new Uri(requestUrl);
		return requestUri.Host + requestUri.AbsolutePath;
	}

	private static RequestMapping? GetResourceMapping(string requestPath, DomainMappedStaticFileOptions options)
		=> options.RequestMappings.FirstOrDefault(m => requestPath.StartsWith(m.RequestUrlPrefix, StringComparison.OrdinalIgnoreCase));
}
