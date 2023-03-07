namespace PanoramicData.Extensions.DomainMappedStaticFiles;

/// <summary>
/// Resource path calculator, converting a request URL to an expected resource location
/// </summary>
internal static class ResourcePathCalculator
{
	/// <summary>
	/// Determine the path to the resource that is being requested
	/// </summary>
	/// <param name="interceptionContext">The context of the interception being performed</param>
	/// <returns>The expected path to the requested resource</returns>
	internal static string GetResourcePath(IInterceptionContext interceptionContext)
	{
		ArgumentNullException.ThrowIfNull(interceptionContext);
		ArgumentNullException.ThrowIfNull(interceptionContext.RequestMapping, nameof(interceptionContext.RequestMapping));

		var requestPath = interceptionContext.RequestPath;
		var mapping = interceptionContext.RequestMapping;

		var subPath = requestPath[mapping.RequestUrlPrefix.Length..];
		if (subPath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			subPath = subPath[1..];
		}

		var resourcePath = mapping.ResourcePath;
		if (!resourcePath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			resourcePath += "/";
		}

		return resourcePath + subPath;
	}
}
