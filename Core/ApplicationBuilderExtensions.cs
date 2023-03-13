using PanoramicData.Extensions.DomainSpecificStaticFiles;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for the IApplicationBuilder interface
/// </summary>
public static class ApplicationBuilderExtensions
{
	/// <summary>
	/// Add the domain specific static file middleware to the request pipeline
	/// </summary>
	/// <param name="app">The instance of the interface that we are extending</param>
	/// <returns>The instance that was extended, to support method chaining</returns>
	public static IApplicationBuilder UseDomainSpecificStaticFiles(this IApplicationBuilder app)
	{
		_ = app.UseMiddleware<DomainSpecificStaticFileMiddleware>();

		return app;
	}
}
