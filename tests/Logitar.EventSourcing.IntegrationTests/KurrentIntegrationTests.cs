using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;
using Logitar.EventSourcing.Kurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing;

public abstract class KurrentIntegrationTests
{
  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected EventBusMock EventBus { get; } = new();
  protected IEventSerializer EventSerializer { get; }
  protected EventStoreClient EventStoreClient { get; }

  protected KurrentIntegrationTests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(Configuration);
    services.AddSingleton<IEventBus>(EventBus);

    string connectionString = Configuration.GetConnectionString("EventStoreDB")
      ?? throw new InvalidOperationException("The connection string 'EventStoreDB' is required.");
    services.AddLogitarEventSourcingWithKurrent(connectionString);

    ServiceProvider = services.BuildServiceProvider();

    EventSerializer = ServiceProvider.GetRequiredService<IEventSerializer>();
    EventStoreClient = ServiceProvider.GetRequiredService<EventStoreClient>();
  }
}
