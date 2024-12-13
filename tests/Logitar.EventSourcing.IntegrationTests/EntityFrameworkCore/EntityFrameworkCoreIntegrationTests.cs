using Bogus;
using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.EntityFrameworkCore.SqlServer;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.EntityFrameworkCore;

public abstract class EntityFrameworkCoreIntegrationTests : IAsyncLifetime
{
  private readonly DatabaseProvider _databaseProvider;

  protected CancellationToken CancellationToken { get; } = default;
  protected Faker Faker { get; } = new();

  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected EventBusMock EventBus { get; } = new();
  protected EventContext EventContext { get; }
  protected IEventConverter EventConverter { get; }

  protected EntityFrameworkCoreIntegrationTests(DatabaseProvider databaseProvider)
  {
    _databaseProvider = databaseProvider;

    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(Configuration);
    services.AddSingleton<IEventBus>(EventBus);

    string connectionString = Configuration.GetConnectionString(databaseProvider.ToString())
      ?.Replace("{Database}", GetType().Name)
      ?? throw new InvalidOperationException($"The connection string '{databaseProvider}' is required.");
    switch (databaseProvider)
    {
      case DatabaseProvider.PostgreSQL:
        services.AddLogitarEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString);
        break;
      case DatabaseProvider.SqlServer:
        services.AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(connectionString);
        break;
    }

    ServiceProvider = services.BuildServiceProvider();

    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    EventConverter = ServiceProvider.GetRequiredService<IEventConverter>();
  }

  public async Task InitializeAsync()
  {
    await EventContext.Database.MigrateAsync(CancellationToken);

    StringBuilder sql = new();
    TableId[] tables = [EventDb.Streams.Table];
    foreach (TableId table in tables)
    {
      IDeleteBuilder? delete = GetDeleteBuilder(table);
      if (delete != null)
      {
        sql.AppendLine(delete.Build().Text);
      }
    }
    await EventContext.Database.ExecuteSqlRawAsync(sql.ToString(), CancellationToken);
  }
  protected virtual IDeleteBuilder? GetDeleteBuilder(TableId table) => _databaseProvider switch
  {
    DatabaseProvider.PostgreSQL => PostgresDeleteBuilder.From(table),
    DatabaseProvider.SqlServer => SqlServerDeleteBuilder.From(table),
    _ => null,
  };

  public Task DisposeAsync() => Task.CompletedTask;
}
