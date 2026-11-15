using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Interfaces;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Fakes;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests;

public class ServiceRegistrationTests
{
	private static ServiceCollection CreateServices()
	{
		var services = new ServiceCollection();
		services.AddLogging();
		return services;
	}

	private static DomainSpecificStaticFileOptions ExistingFileOptions(string relativeUrlPrefix = "/style/")
	{
		var options = MappedOptionsFactory.Create(relativeUrlPrefix);
		options.RegisterFileProvider<ExistingResourceProvider>();
		return options;
	}

	private static async Task<int> RequestStatusAsync(ServiceProvider provider, string path)
	{
		var context = MappedOptionsFactory.CreateRequest(path);
		await provider.GetRequiredService<DomainSpecificStaticFileMiddleware>().InvokeAsync(context, _ => Task.CompletedTask);
		return context.Response.StatusCode;
	}

	private static bool HasMappings(IMappingConfigurationProvider provider)
		=> provider.GetConfiguration().DomainResourceMappings.Count > 0;

	[Fact]
	public async Task AddDomainSpecificStaticFiles_Options_InterceptsMatchingRequest()
	{
		var services = CreateServices();
		services.AddDomainSpecificStaticFiles(new ConfigurationBuilder().Build(), options =>
		{
			options.RequestMappings = ExistingFileOptions().RequestMappings;
			options.RegisterFileProvider<ExistingResourceProvider>();
		});
		await using var provider = services.BuildServiceProvider();

		(await RequestStatusAsync(provider, "/style/a.png")).Should().Be(StatusCodes.Status200OK);
	}

	[Fact]
	public async Task AddDomainSpecificStaticFiles_ConfigurationOverride_ReplacesRequestMappings()
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["DomainMappings:RequestMappings:0:RelativeUrlPrefix"] = "/override/",
				["DomainMappings:RequestMappings:0:ResourcePathPrefix"] = "Override/",
				["DomainMappings:RequestMappings:0:DomainMappings:0:Domain"] = "localhost",
				["DomainMappings:RequestMappings:0:DomainMappings:0:ResourceSubPath"] = "Default",
			})
			.Build();
		var services = CreateServices();
		services.AddDomainSpecificStaticFiles(configuration, options =>
		{
			options.RequestMappings = ExistingFileOptions().RequestMappings;
			options.RegisterFileProvider<ExistingResourceProvider>();
		});
		await using var provider = services.BuildServiceProvider();
		var nextCalled = false;
		var context = MappedOptionsFactory.CreateRequest("/style/a.png");

		await provider.GetRequiredService<DomainSpecificStaticFileMiddleware>().InvokeAsync(context, _ =>
		{
			nextCalled = true;
			return Task.CompletedTask;
		});

		nextCalled.Should().BeTrue();
		(await RequestStatusAsync(provider, "/override/a.png")).Should().Be(StatusCodes.Status200OK);
	}

	[Fact]
	public async Task AddDomainSpecificStaticFiles_Deferred_AppliesRetrievedOptions()
	{
		var services = CreateServices();
		services.AddDomainSpecificStaticFiles(_ => Task.FromResult(ExistingFileOptions()));
		await using var provider = services.BuildServiceProvider();
		var configurationProvider = provider.GetRequiredService<IMappingConfigurationProvider>();

		provider.GetRequiredService<MappingConfigurationProviderInitializer>().StartInitialization();
		var applied = await WaitHelper.UntilAsync(() => HasMappings(configurationProvider));

		applied.Should().BeTrue();
		(await RequestStatusAsync(provider, "/style/a.png")).Should().Be(StatusCodes.Status200OK);
	}

	[Fact]
	public async Task AddDomainSpecificStaticFiles_DeferredFailure_AppliesEmptyConfiguration()
	{
		var services = CreateServices();
		services.AddDomainSpecificStaticFiles(_ => throw new InvalidOperationException("boom"));
		await using var provider = services.BuildServiceProvider();

		provider.GetRequiredService<MappingConfigurationProviderInitializer>().StartInitialization();
		await Task.Delay(200, TestContext.Current.CancellationToken);

		HasMappings(provider.GetRequiredService<IMappingConfigurationProvider>()).Should().BeFalse();
	}

	[Fact]
	public async Task AddDomainSpecificStaticFiles_ProviderInstance_UsesThatInstance()
	{
		var instance = new FixedProvider();
		var services = CreateServices();
		services.AddDomainSpecificStaticFiles(instance);
		await using var provider = services.BuildServiceProvider();

		provider.GetRequiredService<IMappingConfigurationProvider>().Should().BeSameAs(instance);
	}

	[Fact]
	public async Task AddDomainSpecificStaticFiles_ProviderType_RegistersProviderType()
	{
		var services = CreateServices();
		services.AddDomainSpecificStaticFiles<FixedProvider>();
		await using var provider = services.BuildServiceProvider();

		provider.GetRequiredService<IMappingConfigurationProvider>().Should().BeOfType<FixedProvider>();
	}

	[Fact]
	public async Task UseDomainSpecificStaticFiles_ApplicationStarted_InitialisesProvider()
	{
		using var lifetime = new FakeHostApplicationLifetime();
		var services = CreateServices();
		services.AddSingleton<IHostApplicationLifetime>(lifetime);
		services.AddDomainSpecificStaticFiles<FixedProvider>();
		await using var provider = services.BuildServiceProvider();
		var configurationProvider = provider.GetRequiredService<IMappingConfigurationProvider>();

		var app = new ApplicationBuilder(provider);
		app.UseDomainSpecificStaticFiles().Should().BeSameAs(app);
		lifetime.NotifyStarted();
		var applied = await WaitHelper.UntilAsync(() => HasMappings(configurationProvider));

		applied.Should().BeTrue();
	}

	private sealed class ExistingResourceProvider : FakeResourceProvider
	{
		public ExistingResourceProvider() : base(true)
		{
		}
	}

	private sealed class FixedProvider : MappingConfigurationProviderBase
	{
		protected override Task InitializeAsync(IServiceScopeFactory scopeFactory)
		{
			ApplyConfiguration(MappedOptionsFactory.Create());
			return Task.CompletedTask;
		}
	}
}