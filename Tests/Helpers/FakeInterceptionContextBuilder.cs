using Microsoft.AspNetCore.Http;
using PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Fakes;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;

internal class FakeInterceptionContextBuilder
{
	private string _requestPath = "localhost/style/images/favicon.png";
	private DomainMappedStaticFileConfiguration _configuration = DomainMappedStaticFileConfigurationFactory.CreateDefaultConfiguration();
	private HttpContext _httpContext = FakeHttpContextFactory.CreateDefaultHttpContext();

	private FakeInterceptionContextBuilder() { }

	public static FakeInterceptionContextBuilder CreateBuilder()
		=> new();

	public FakeInterceptionContextBuilder ForRequestPath(string requestPath)
	{
		_requestPath = requestPath;
		return this;
	}

	public FakeInterceptionContextBuilder WithConfiguration(DomainMappedStaticFileConfiguration configuration)
	{
		_configuration = configuration;
		return this;
	}

	public FakeInterceptionContextBuilder WithHttpContext(HttpContext context)
	{
		_httpContext = context;
		return this;
	}

	public IInterceptionContext Build() => new FakeInterceptionContext()
	{
		RequestPath = _requestPath,
		Configuration = _configuration,
		HttpContext = _httpContext,
		ResourceMapping = _configuration.GetResourceMapping(_requestPath),
	};
}
