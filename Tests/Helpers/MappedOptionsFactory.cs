using Microsoft.AspNetCore.Http;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

internal static class MappedOptionsFactory
{
	internal static DomainSpecificStaticFileOptions Create(string relativeUrlPrefix = "/style/")
		=> new()
		{
			RequestMappings =
			[
				new RequestMapping(relativeUrlPrefix, "Style/")
				{
					DomainMappings = [new DomainMapping { Domain = "localhost", ResourceSubPath = "Default" }],
				},
			],
		};

	internal static DefaultHttpContext CreateRequest(string path)
	{
		var context = new DefaultHttpContext();
		context.Request.Scheme = "https";
		context.Request.Host = new HostString("localhost");
		context.Request.Path = path;
		context.Response.Body = new MemoryStream();
		return context;
	}
}