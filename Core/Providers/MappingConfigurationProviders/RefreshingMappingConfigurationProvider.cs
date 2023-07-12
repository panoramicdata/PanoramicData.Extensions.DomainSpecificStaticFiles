using System.Diagnostics;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;

/// <summary>
/// Refreshing mapping configuration provider, which enables the running of an async
/// task to get the configuration options on a regular schedule, including at initialization
/// </summary>
public class RefreshingMappingConfigurationProvider : MappingConfigurationProviderBase
{
	private readonly Func<CancellationToken, Task<DomainSpecificStaticFileOptions>> _retrievalTask;
	private readonly TimeSpan _refreshInterval;
	private readonly IHostApplicationLifetime _hostApplicationLifetime;
	private CancellationTokenSource? _cancellationTokenSource;

	public RefreshingMappingConfigurationProvider(Func<CancellationToken, Task<DomainSpecificStaticFileOptions>> retrievalTask, TimeSpan refreshInterval, IHostApplicationLifetime hostApplicationLifetime)
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
	protected override async Task InitializeAsync()
	{
		_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_hostApplicationLifetime.ApplicationStopping);

		TimeSpan timeTaken;
		try
		{
			timeTaken = await RetrieveAsync(_cancellationTokenSource.Token);
		}
		catch (TaskCanceledException)
		{
			return;
		}

		// Start the process of performing regular refreshes
		_ = RunRefreshAsync(timeTaken);
	}

	private async Task<TimeSpan> RetrieveAsync(CancellationToken cancellationToken)
	{
		var startTimestamp = Stopwatch.GetTimestamp();

		var options = await _retrievalTask(cancellationToken);
		ApplyConfiguration(options);

		return Stopwatch.GetElapsedTime(startTimestamp);
	}

	private async Task RunRefreshAsync(TimeSpan timeTaken)
	{

		try
		{
			var cancellationToken = _cancellationTokenSource!.Token;
			while (!cancellationToken.IsCancellationRequested)
			{
				await DelayUntilNextRefresh(timeTaken, cancellationToken);
				timeTaken = await RetrieveAsync(cancellationToken);
			}
		}
		catch (TaskCanceledException)
		{
			// No need to do anything; cancellation is intentionally supported
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
