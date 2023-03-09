using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Fakes;

internal class FakeInterceptionContext : IInterceptionContext
{
	public bool CanInterceptRequest => ResourceMapping is not null;

	public required HttpContext HttpContext { get; set; }

	public required DomainMappedStaticFileConfiguration Configuration { get; set; }

	public string RequestPath { get; set; } = string.Empty;

	public ResourceMapping? ResourceMapping { get; set; }
}
