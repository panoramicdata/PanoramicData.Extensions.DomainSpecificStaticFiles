namespace PanoramicData.Extensions.DomainMappedStaticFiles;

public interface IInterceptionContext
{
	/// <summary>
	/// Can this request be intercepted
	/// </summary>
	bool CanInterceptRequest { get; }

	/// <summary>
	/// The context of the Http request that is being handled
	/// </summary>
	HttpContext HttpContext { get; }

	/// <summary>
	/// The request that is being handled
	/// </summary>
	HttpRequest Request { get; }

	/// <summary>
	/// The response that is being generated
	/// </summary>
	HttpResponse Response { get; }

	/// <summary>
	/// The configuration of the domain mapping subsystem
	/// </summary>
	DomainMappedStaticFileConfiguration Configuration { get; }

	/// <summary>
	/// The path of the request that is used for resource location
	/// </summary>
	string RequestPath { get; }

	/// <summary>
	/// The lowered path of the request that is used for lookups
	/// </summary>
	string LoweredRequestPath { get; }

	/// <summary>
	/// The resource mappings that we are to use in handling requests
	/// </summary>
	ResourceMapping? ResourceMapping { get; }
}