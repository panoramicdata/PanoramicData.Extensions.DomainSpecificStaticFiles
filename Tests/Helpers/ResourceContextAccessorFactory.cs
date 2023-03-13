using PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Fakes;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;

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
