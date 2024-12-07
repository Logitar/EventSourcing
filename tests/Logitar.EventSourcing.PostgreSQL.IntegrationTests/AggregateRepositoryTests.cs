using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Logitar.EventSourcing.PostgreSQL;

[Trait(Traits.Category, Categories.Integration)]
public class AggregateRepositoryTests : Relational.AggregateRepositoryTests
{
  public AggregateRepositoryTests() : base()
  {
  }

  protected override IInsertBuilder CreateInsertBuilder(params ColumnId[] columns)
  {
    return new PostgresInsertBuilder(columns);
  }
  protected override IQueryBuilder CreateQueryBuilder(TableId source)
  {
    return new PostgresQueryBuilder(source);
  }

  protected override Relational.AggregateRepository CreateRepository(IEventBus eventBus)
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    string connectionString = configuration.GetValue<string>("POSTGRESQLCONNSTR_IntegrationTests")
      ?? throw new InvalidOperationException("The configuration 'POSTGRESQLCONNSTR_IntegrationTests' has not been set.");

    Connection = new NpgsqlConnection(connectionString);

    return new AggregateRepository(Connection, eventBus, EventSerializer);
  }

  protected override async Task RecreateDatabaseAsync(CancellationToken cancellationToken)
  {
    Assert.NotNull(Connection);

    string script = await File.ReadAllTextAsync("Init.sql", Encoding.UTF8, cancellationToken);
    string[] statements = script.Split("GO");

    using DbCommand command = Connection.CreateCommand();
    foreach (string statement in statements)
    {
      command.CommandText = statement;
      await command.ExecuteNonQueryAsync(cancellationToken);
    }
  }
}
