using Microsoft.AspNetCore.Http;
using PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Fakes;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;

internal class FakeInterceptionContextBuilder
{
	private string _requestPath = "localhost/style/images/favicon.png";
	private DomainMappedStaticFileOptions _options = DomainMappedStaticFileOptionsFactory.CreateDefaultOptions();
	private HttpContext _httpContext = FakeHttpContextFactory.CreateDefaultHttpContext();
	private RequestMapping? _requestMapping;

	private FakeInterceptionContextBuilder() { }

	public static FakeInterceptionContextBuilder CreateBuilder()
		=> new();

	public FakeInterceptionContextBuilder ForRequestPath(string requestPath)
	{
		_requestPath = requestPath;
		return this;
	}

	public FakeInterceptionContextBuilder WithOptions(DomainMappedStaticFileOptions options)
	{
		_options = options;
		return this;
	}

	public FakeInterceptionContextBuilder WithRequestMappingAtIndex(int index)
	{
		var mappings = new List<RequestMapping>(_options.RequestMappings);
		_requestMapping = mappings[index];
		return this;
	}

	public FakeInterceptionContextBuilder WithHttpContext(HttpContext context)
	{
		_httpContext = context;
		return this;
	}

	public IInterceptionContext Build()
		=> new FakeInterceptionContext()
		{
			RequestPath = _requestPath,
			Options = _options,
			HttpContext = _httpContext,
			RequestMapping = _requestMapping
		};
}
