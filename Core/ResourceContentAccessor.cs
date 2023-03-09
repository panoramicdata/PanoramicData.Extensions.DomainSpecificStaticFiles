namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// Accessor for retrieving the contents of a domain mapped resource
/// </summary>
public class ResourceContentAccessor : IDisposable
{
	private bool _disposedValue;

	/// <summary>
	/// The stream for accessing the contents of the resource
	/// </summary>
	public required Stream ResourceStream { get; init; }

	/// <summary>
	/// The MIME type of the resource
	/// </summary>
	public required string MimeType { get; init; }

	/// <summary>
	/// The hash of the resource
	/// </summary>
	public required string Hash { get; init; }

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
				ResourceStream.Dispose();
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