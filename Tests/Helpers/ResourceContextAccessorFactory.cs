using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Fakes;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

internal static class ResourceContextAccessorFactory
{
	public static ResourceContentAccessor CreateDefaultContentAccessor()
	{
		var resourceInfo = new ResourceInfo()
		{
			ResourcePath = "localhost/images/favicon.png",
			Exists = true
		};
		return new ResourceContentAccessor
		{
			MimeType = "image/png",
			ResourceInfo = resourceInfo,
			ResourceProvider = new FakeResourceProvider(true)
		};
	}
}
