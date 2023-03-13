using Microsoft.Extensions.Logging;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Fakes;

internal class FakeLogger<T> : ILogger<T>
{
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => new FakeScope();
	public bool IsEnabled(LogLevel logLevel) => false;
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
}
