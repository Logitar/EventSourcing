using Logitar.EventSourcing.Infrastructure;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace Logitar.EventSourcing.MartenDB;

public abstract class MartenIntegrationTests
{
  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected IDocumentStore DocumentStore { get; }
  protected EventBusMock EventBus { get; } = new();
  protected IEventSerializer EventSerializer { get; }

  protected MartenIntegrationTests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(Configuration);
    services.AddSingleton<IEventBus>(EventBus);

    var connectionString = Configuration.GetConnectionString("PostgreSQL")?.Replace("{Database}", GetType().Name)
      ?? throw new InvalidOperationException("The connection string 'PostgreSQL' is required.");
    #region TODO(fpion): refactor
    //services.AddLogitarEventSourcingWithMarten(connectionString);
    services.AddLogitarEventSourcingInfrastructure();
    services.AddMarten(options =>
    {
      options.Connection(connectionString);
      options.UseSystemTextJsonForSerialization();
      options.AutoCreateSchemaObjects = AutoCreate.All;
    });
    //services.AddScoped<IEventStore, MartenStore>();
    #endregion

    ServiceProvider = services.BuildServiceProvider();

    DocumentStore = ServiceProvider.GetRequiredService<IDocumentStore>();
    EventSerializer = ServiceProvider.GetRequiredService<IEventSerializer>();
  }
}
