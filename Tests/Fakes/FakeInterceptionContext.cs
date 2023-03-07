using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Fakes;

internal class FakeInterceptionContext : IInterceptionContext
{
	public bool CanInterceptRequest => RequestMapping is not null;

	public required HttpContext HttpContext { get; set; }

	public required DomainMappedStaticFileOptions Options { get; set; }

	public string RequestPath { get; set; } = string.Empty;

	public RequestMapping? RequestMapping { get; set; }
}
