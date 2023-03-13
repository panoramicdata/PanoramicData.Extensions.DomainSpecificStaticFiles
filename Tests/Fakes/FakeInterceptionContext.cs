using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Fakes;

internal class FakeInterceptionContext : IInterceptionContext
{
	public bool CanInterceptRequest => ResourceMapping is not null;

	public required HttpContext HttpContext { get; set; }

	public HttpRequest Request => HttpContext.Request;

	public HttpResponse Response => HttpContext.Response;

	public required DomainSpecificStaticFileConfiguration Configuration { get; set; }

	public string RequestPath { get; set; } = string.Empty;

	public string LoweredRequestPath => RequestPath.ToLowerInvariant();

	public ResourceMapping? ResourceMapping { get; set; }
}
