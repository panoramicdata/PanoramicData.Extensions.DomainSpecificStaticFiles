using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Interfaces;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Providers.MappingConfigurationProviders;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Fakes;
using PanoramicData.Extensions.DomainSpecificStaticFiles.Tests.Helpers;

namespace PanoramicData.Extensions.DomainSpecificStaticFiles.Tests;

public class RefreshingProviderTests
{
	private static Task<DomainSpecificStaticFileOptions> Retrieve(IServiceScopeFactory _, CancellationToken __)
		=> Task.FromResult(MappedOptionsFactory.Create());

	private static async Task<bool> InitialiseAsync(
		Func<IServiceScopeFactory, CancellationToken, Task<DomainSpecificStaticFileOptions>> retrieval,
		Func<IMappingConfigurationProvider, bool> condition)
	{
		using var lifetime = new FakeHostApplicationLifetime();
		using var provider = new RefreshingMappingConfigurationProvider(retrieval, TimeSpan.FromSeconds(15), lifetime);
		await using var services = new ServiceCollection().BuildServiceProvider();
		var mappingProvider = (IMappingConfigurationProvider)provider;

		var initial = mappingProvider.GetConfiguration();
		mappingProvider.StartInitialization(services.GetRequiredService<IServiceScopeFactory>());
		var reached = await WaitHelper.UntilAsync(() => condition(mappingProvider) && !ReferenceEquals(mappingProvider.GetConfiguration(), initial));
		await Task.Delay(100, TestContext.Current.CancellationToken);
		lifetime.StopApplication();

		return reached;
	}

	[Theory]
	[InlineData(14, 30)]
	[InlineData(15, 4)]
	public void Constructor_IntervalTooShort_Throws(int refreshSeconds, int retrySeconds)
	{
		using var lifetime = new FakeHostApplicationLifetime();

		var action = () => new RefreshingMappingConfigurationProvider(Retrieve, TimeSpan.FromSeconds(refreshSeconds), TimeSpan.FromSeconds(retrySeconds), lifetime);

		action.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public async Task StartInitialization_RetrievalSucceeds_AppliesConfiguration()
	{
		var applied = await InitialiseAsync(Retrieve, p => p.GetConfiguration().DomainResourceMappings.Count == 1);

		applied.Should().BeTrue();
	}

	[Fact]
	public async Task StartInitialization_RetrievalFails_StillAttemptsRetrievalAndKeepsEmptyConfiguration()
	{
		var attempted = false;
		Task<DomainSpecificStaticFileOptions> Fail(IServiceScopeFactory _, CancellationToken __)
		{
			attempted = true;
			throw new InvalidOperationException("boom");
		}

		var tried = await InitialiseAsync(Fail, _ => attempted);

		tried.Should().BeTrue();
	}
}