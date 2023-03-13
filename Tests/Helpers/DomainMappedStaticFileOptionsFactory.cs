namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;
internal static class DomainMappedStaticFileOptionsFactory
{
	public static DomainMappedStaticFileOptions CreateDefaultOptions()
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

	public static DomainMappedStaticFileOptions CreateOptions(IEnumerable<RequestMapping> requestMappings)
		=> new()
		{
			RequestMappings = requestMappings
		};
}
