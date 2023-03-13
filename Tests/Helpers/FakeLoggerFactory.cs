using Microsoft.Extensions.Logging;
using PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Fakes;

namespace PanoramicData.Extensions.DomainMappedStaticFiles.Tests.Helpers;
internal static class FakeLoggerFactory
{
	public static ILogger<T> CreateDefaultLogger<T>() => new FakeLogger<T>();
}
