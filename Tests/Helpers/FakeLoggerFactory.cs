using Microsoft.Extensions.Logging;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Fakes;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;
internal static class FakeLoggerFactory
{
	public static ILogger<T> CreateDefaultLogger<T>() => new FakeLogger<T>();
}
