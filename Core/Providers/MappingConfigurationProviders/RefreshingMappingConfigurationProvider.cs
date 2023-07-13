using System.Diagnostics;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;

/// <summary>
/// Refreshing mapping configuration provider, which enables the running of an async
/// task to get the configuration options on a regular schedule, including at initialization
/// </summary>
public class RefreshingMappingConfigurationProvider : MappingConfigurationProviderBase
{
	private readonly Func<IServiceScopeFactory, CancellationToken, Task<DomainSpecificStaticFileOptions>> _retrievalTask;
	private readonly TimeSpan _refreshInterval;
	private readonly IHostApplicationLifetime _hostApplicationLifetime;
	private IServiceScopeFactory? _scopeFactory;
	private CancellationTokenSource? _cancellationTokenSource;

	public RefreshingMappingConfigurationProvider(Func<IServiceScopeFactory, CancellationToken, Task<DomainSpecificStaticFileOptions>> retrievalTask, TimeSpan refreshInterval, IHostApplicationLifetime hostApplicationLifetime)
	{
		_retrievalTask = retrievalTask;
		_refreshInterval = refreshInterval;
		_hostApplicationLifetime = hostApplicationLifetime;

		if (refreshInterval < TimeSpan.FromMinutes(1))
		{
			throw new ArgumentOutOfRangeException(nameof(refreshInterval), "Refreshing more frequently than every minute is not supported!");
		}
	}

	/// <summary>
	/// Async initializer, which makes use of the provided task to get the configuration options
	/// </summary>
	protected override async Task InitializeAsync(IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory;
		_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping);

		TimeSpan timeTaken;
		try
		{
			timeTaken = await RetrieveAsync(_scopeFactory, _cancellationTokenSource.Token);
		}
		catch (TaskCanceledException)
		{
			return;
		}

		// Start the process of performing regular refreshes
		_ = RunRefreshAsync(timeTaken);
	}

	private async Task<TimeSpan> RetrieveAsync(IServiceScopeFactory scopeFactory, CancellationToken cancellationToken)
	{
		if (_scopeFactory is null)
		{
			// Initialization is not yet done; we can do no work here
			return TimeSpan.Zero;
		}

		var startTimestamp = Stopwatch.GetTimestamp();

		var options = await _retrievalTask(scopeFactory, cancellationToken);
		ApplyConfiguration(options);

		return Stopwatch.GetElapsedTime(startTimestamp);
	}

	private async Task RunRefreshAsync(TimeSpan timeTaken)
	{
		if (_scopeFactory is null)
		{
			// Initialization has not completed, so we cannot refresh
			return;
		}

		try
		{
			var cancellationToken = _cancellationTokenSource!.Token;
			while (!cancellationToken.IsCancellationRequested)
			{
				await DelayUntilNextRefresh(timeTaken, cancellationToken);
				timeTaken = await RetrieveAsync(_scopeFactory, cancellationToken);
			}
		}
		catch (TaskCanceledException)
		{
			// No need to do anything; cancellation is intentionally supported
		}
		catch (Exception ex)
		{
			using var scope = _scopeFactory.CreateScope();
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<RefreshingMappingConfigurationProvider>>();
			logger.LogError(ex, "Exception during mapping configuration retrieval. Message: {Message}", ex.Message);
		}
	}

	private async Task DelayUntilNextRefresh(TimeSpan timeTaken, CancellationToken cancellationToken)
	{
		var delayPeriod = _refreshInterval;
		if (timeTaken < _refreshInterval)
		{
			delayPeriod = _refreshInterval - timeTaken;
		}

		await Task.Delay(delayPeriod, cancellationToken);
	}
}
