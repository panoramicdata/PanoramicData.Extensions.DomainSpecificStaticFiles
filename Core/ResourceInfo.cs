namespace PanoramicData.Extensions.DomainMappedStaticFiles;
/// <summary>
/// Information about a white labelled resource
/// </summary>
public class ResourceInfo
{
	/// <summary>
	/// Whether the resource exists
	/// </summary>
	public bool Exists { get; init; }

	/// <summary>
	/// The path of the resource in the format needed by the file provider
	/// </summary>
	public string ResourcePath { get; init; } = string.Empty;

	/// <summary>
	/// The hash of the contents of the resource
	/// </summary>
	public string Hash { get; init; } = string.Empty;
}