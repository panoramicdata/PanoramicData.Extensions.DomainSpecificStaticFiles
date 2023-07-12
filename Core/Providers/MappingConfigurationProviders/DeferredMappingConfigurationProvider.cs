namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;

/// <summary>
/// Deferred mapping configuration provider, which enables the running of an async
/// task to get the configuration options just once, during initialization
/// </summary>
internal class DeferredMappingConfigurationProvider : MappingConfigurationProviderBase
{
	private readonly Func<Task<DomainSpecificStaticFileOptions>> _initializationTask;

	public DeferredMappingConfigurationProvider(Func<Task<DomainSpecificStaticFileOptions>> initializationTask)
	{
		_initializationTask = initializationTask;
	}

	/// <summary>
	/// Async initializer, which makes use of the provided task to get the configuration options
	/// </summary>
	protected override async Task InitializeAsync()
	{
		var options = await _initializationTask();
		ApplyConfiguration(options);
	}
}
