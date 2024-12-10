using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;
using Logitar.EventSourcing.Kurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing;

public abstract class IntegrationTests
{
  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected EventBusMock EventBus { get; } = new();
  protected IEventSerializer EventSerializer { get; }
  protected EventStoreClient EventStoreClient { get; }

  protected IntegrationTests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(Configuration);
    services.AddSingleton<IEventBus>(EventBus);

    string connectionString = Configuration.GetValue<string>("ESDBCONNSTR_IntegrationTests")
      ?? throw new InvalidOperationException("The configuration 'ESDBCONNSTR_IntegrationTests' is missing.");
    services.AddLogitarEventSourcingWithKurrent(connectionString);

    ServiceProvider = services.BuildServiceProvider();

    EventSerializer = ServiceProvider.GetRequiredService<IEventSerializer>();
    EventStoreClient = ServiceProvider.GetRequiredService<EventStoreClient>();
  }
}
