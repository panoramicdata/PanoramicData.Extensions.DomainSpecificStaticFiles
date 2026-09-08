using PanoramicData.Extensions.DomainSpecificStaticFiles.Interfaces;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;

/// <summary>
/// Base class for mapping configuration providers, handling the asynchronous initialization
/// and thread-safe replacement of the runtime configuration
/// </summary>
public abstract class MappingConfigurationProviderBase : IMappingConfigurationProvider, IDisposable
{
	private readonly object _updateLock = new();
	private Task? _initializationTask;
	private IServiceScopeFactory? _scopeFactory;
	private DomainSpecificStaticFileConfiguration _configuration = new();
	private bool _configurationApplied;

	/// <summary>
	/// Start async initialization of configuration
	/// </summary>
	public void StartInitialization(IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory;

		// Set up for async initialization, but don't start the operation yet
		_initializationTask = new Task(async () => await InitializeAsync(scopeFactory));
		_ = _initializationTask.ContinueWith(_ => InitializationCompletedHandler(), TaskScheduler.Default);

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
	/// <exception cref="InvalidOperationException">The mapping configuration provider did not apply config during initialization</exception>
	private void InitializationCompletedHandler()
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

	/// <summary>
	/// Handle logging of an exception, if logging has been enabled
	/// </summary>
	/// <param name="ex">The exception that is to be logged</param>
	protected void LogException(Exception ex)
	{
		if (_scopeFactory is null)
		{
			return;
		}

		try
		{
			using var scope = _scopeFactory.CreateScope();
			var logger = scope.ServiceProvider.GetService<ILogger<RefreshingMappingConfigurationProvider>>();
			logger?.LogError(ex, "Exception during mapping configuration retrieval. Message: {Message}", ex.Message);
		}
		catch (Exception)
		{
			// Not much we can do about a logging failure, other than avoid it being rethrown
		}
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_initializationTask?.Dispose();
		GC.SuppressFinalize(this);
	}
}
