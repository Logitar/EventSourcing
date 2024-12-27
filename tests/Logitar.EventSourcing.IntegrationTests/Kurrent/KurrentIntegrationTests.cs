using Bogus;
using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Kurrent;

public abstract class KurrentIntegrationTests
{
  protected CancellationToken CancellationToken { get; } = default;
  protected Faker Faker { get; } = new();

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
    services.AddLogitarEventSourcing();
    services.AddLogitarEventSourcingInfrastructure();
    services.AddLogitarEventSourcingWithKurrent(connectionString);

    ServiceProvider = services.BuildServiceProvider();

    EventSerializer = ServiceProvider.GetRequiredService<IEventSerializer>();
    EventStoreClient = ServiceProvider.GetRequiredService<EventStoreClient>();
  }
}
