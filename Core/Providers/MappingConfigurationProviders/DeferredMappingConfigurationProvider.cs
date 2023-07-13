namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;

/// <summary>
/// Deferred mapping configuration provider, which enables the running of an async
/// task to get the configuration options just once, during initialization
/// </summary>
internal class DeferredMappingConfigurationProvider : MappingConfigurationProviderBase
{
	private readonly Func<IServiceScopeFactory, Task<DomainSpecificStaticFileOptions>> _initializationTask;

	public DeferredMappingConfigurationProvider(Func<IServiceScopeFactory, Task<DomainSpecificStaticFileOptions>> initializationTask)
	{
		_initializationTask = initializationTask;
	}

	/// <summary>
	/// Async initializer, which makes use of the provided task to get the configuration options
	/// </summary>
	protected override async Task InitializeAsync(IServiceScopeFactory scopeFactory)
	{
		var options = await _initializationTask(scopeFactory);
		ApplyConfiguration(options);
	}
}
