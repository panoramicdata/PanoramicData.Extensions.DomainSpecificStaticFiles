namespace PanoramicData.Extensions.DomainSpecificStaticFiles;
/// <summary>
/// Information about a domain-specific resource
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
}