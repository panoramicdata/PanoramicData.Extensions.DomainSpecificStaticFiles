namespace PanoramicData.Extensions.DomainMappedStaticFiles.Demo;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddRazorPages();
		builder.Services.AddServerSideBlazor();

		builder.Services.AddDomainMappedStaticFiles(builder.Configuration, GetDomainMappedStaticFileOptionsAction());

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
		app.UseDomainMappedStaticFiles();

		app.UseRouting();

		app.MapBlazorHub();
		app.MapFallbackToPage("/_Host");

		app.Run();
	}

	private static Action<DomainMappedStaticFileOptions> GetDomainMappedStaticFileOptionsAction()
		=> (options) => options.RequestMappings.Add(
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

