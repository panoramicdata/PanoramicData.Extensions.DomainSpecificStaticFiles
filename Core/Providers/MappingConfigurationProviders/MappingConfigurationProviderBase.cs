using PanoramicData.Extensions.DomainSpecificStaticFiles.Interfaces;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;

public abstract class MappingConfigurationProviderBase : IMappingConfigurationProvider, IDisposable
{
	private readonly object _updateLock = new();
	private Task? _initializationTask;
	private DomainSpecificStaticFileConfiguration _configuration = new();
	private bool _configurationApplied;

	/// <summary>
	/// Start async initialization of configuration
	/// </summary>
	public void StartInitialization(IServiceScopeFactory scopeFactory)
	{
		// Set up for async initialization, but don't start the operation yet
		_initializationTask = new Task(async () => await InitializeAsync(scopeFactory))
			.ContinueWith(InitializationCompletedHandler);

		// All set up; now start the operation; it will continue after we return
		_initializationTask.Start();
	}

	/// <summary>
	/// Method to be implemented by a provider to asynchronously initialize the configuration
	/// </summary>
	/// <param name="scopeFactory">A service scope factory that can be used to start a service scope, if needed</param>
	/// <remarks>
	/// The implementer MUST call ApplyConfiguration to provide the options to the 
	/// underlying system before the completion of the initialization operation.
	/// </remarks>
	protected abstract Task InitializeAsync(IServiceScopeFactory scopeFactory);

	/// <summary>
	/// Handler that is run when async initialization has been completed
	/// </summary>
	/// <param name="task">The task that was performed</param>
	/// <exception cref="InvalidOperationException">The mapping configuration provider did not apply config during initialization</exception>
	private void InitializationCompletedHandler(Task task)
	{
		if (!_configurationApplied)
		{
			throw new InvalidOperationException(
				"Mapping configuration providers MUST call ApplyConfiguration during initialization!");
		}
	}

	/// <summary>
	/// Exposed method for configuration providers to call to supply config
	/// </summary>
	/// <param name="options">The configuration options to be applied</param>
	protected void ApplyConfiguration(DomainSpecificStaticFileOptions options)
	{
		ArgumentNullException.ThrowIfNull(options, nameof(options));

		// Translate the options provided by the developer into perf-optimised runtime configuration
		var configuration = DomainSpecificStaticFileOptionsTranslator.Translate(options);

		// Apply the new configuration
		lock (_updateLock)
		{
			_configuration = configuration;
			_configurationApplied = true;
		}
	}

	/// <summary>
	/// Method used by the subsystem to get the current configuration
	/// </summary>
	/// <returns>The current configuration, as it exists</returns>
	DomainSpecificStaticFileConfiguration IMappingConfigurationProvider.GetConfiguration()
		=> _configuration;

	public void Dispose()
	{
		_initializationTask?.Dispose();
		GC.SuppressFinalize(this);
	}
}
