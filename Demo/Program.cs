namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Demo;

public static class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddRazorPages();
		builder.Services.AddServerSideBlazor();

		builder.Services.AddDomainSpecificStaticFiles(builder.Configuration, ApplyDomainSpecificStaticFileOptions);

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();

		app.UseStaticFiles();
		app.UseDomainSpecificStaticFiles();

		app.UseRouting();

		app.MapBlazorHub();
		app.MapFallbackToPage("/_Host");

		app.Run();
	}

	/// <summary>
	/// Apply the configuration of the request mappings and domains for which to serve static files
	/// </summary>
	/// <param name="options">The instance of the options to which we are to apply configuration</param>
	private static void ApplyDomainSpecificStaticFileOptions(DomainSpecificStaticFileOptions options)
		=> options.RequestMappings.Add(
			new()
			{
				RelativeUrlPrefix = "style",
				ResourcePathPrefix = "Style",
				DomainMappings = new()
				{
					new DomainMapping { Domain = "localhost", ResourceSubPath = "Default" }
				}
			});
}

