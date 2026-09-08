namespace PanoramicData.Extensions.DomainSpecificStaticFiles;

/// <summary>
/// Accessor for retrieving the contents of a domain-specific resource
/// </summary>
public class ResourceContentAccessor : IDisposable
{
	private bool _disposedValue;

	/// <summary>
	/// The information about the resource being accessed
	/// </summary>
	public required ResourceInfo ResourceInfo { get; init; }

	/// <summary>
	/// The provider for accessing the contents of the resource
	/// </summary>
	public required IResourceProvider ResourceProvider { get; init; }

	/// <summary>
	/// The MIME type of the resource
	/// </summary>
	public required string MimeType { get; init; }

	/// <summary>
	/// Get the stream for accessing the resource contents
	/// </summary>
	/// <param name="cancellationToken">The token used to manage cancellation</param>
	/// <returns>A stream providing access to the resource contents</returns>
	public Task<Stream> GetResourceContentStreamAsync(CancellationToken cancellationToken)
		=> ResourceProvider.GetResourceContentStreamAsync(ResourceInfo, cancellationToken);

	/// <summary>
	/// Get the hash of the resource contents
	/// </summary>
	/// <param name="cancellationToken">The token used to manage cancellation</param>
	/// <returns>A hash of the resource contents</returns>
	public Task<byte[]> GetHashAsync(CancellationToken cancellationToken)
		=> ResourceProvider.GetResourceContentsHashAsync(ResourceInfo, cancellationToken);

	/// <summary>
	/// Get the last modified date and time of the resource contents
	/// </summary>
	/// <param name="cancellationToken">The token used to manage cancellation</param>
	/// <returns>The last modified date and time of the resource</returns>
	public Task<DateTime> GetLastModifiedAsync(CancellationToken cancellationToken)
		=> ResourceProvider.GetResourceLastModifiedUtcAsync(ResourceInfo, cancellationToken);

	/// <summary>
	/// Perform disposal of managed resources
	/// </summary>
	/// <param name="disposing">Whether disposal has been initiated</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				if (ResourceProvider is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}

			_disposedValue = true;
		}
	}

	/// <summary>
	/// Dispose of the structure and its dependencies
	/// </summary>
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}