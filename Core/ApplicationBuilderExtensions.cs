using PanoramicData.Extensions.DomainMappedStaticFiles;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for the IApplicationBuilder interface
/// </summary>
public static class ApplicationBuilderExtensions
{
	/// <summary>
	/// Add the domain mapped static file middleware to the request pipeline
	/// </summary>
	/// <param name="app">The instance of the interface that we are extending</param>
	/// <returns>The instance that was extended, to support method chaining</returns>
	public static IApplicationBuilder UseDomainMappedStaticFiles(this IApplicationBuilder app)
	{
		_ = app.UseMiddleware<DomainMappedStaticFileMiddleware>();

		return app;
	}
}
