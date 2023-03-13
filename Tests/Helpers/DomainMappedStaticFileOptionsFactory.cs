namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;
internal static class DomainSpecificStaticFileOptionsFactory
{
	public static DomainSpecificStaticFileOptions CreateDefaultOptions()
		=> new()
		{
			RequestMappings = new List<RequestMapping>()
			{
				new RequestMapping
				{
					RelativeUrlPrefix = "localhost/style/",
					ResourcePathPrefix = "Style/Default/"
				}
			}
		};

	public static DomainSpecificStaticFileOptions CreateOptions(IEnumerable<RequestMapping> requestMappings)
		=> new()
		{
			RequestMappings = new List<RequestMapping>(requestMappings)
		};
}
