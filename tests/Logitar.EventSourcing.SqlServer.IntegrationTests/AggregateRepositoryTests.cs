using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Logitar.EventSourcing.SqlServer;

[Trait(Traits.Category, Categories.Integration)]
public class AggregateRepositoryTests : Relational.AggregateRepositoryTests
{
  public AggregateRepositoryTests() : base()
  {
  }

  protected override IInsertBuilder CreateInsertBuilder(params ColumnId[] columns)
  {
    return new SqlServerInsertBuilder(columns);
  }
  protected override IQueryBuilder CreateQueryBuilder(TableId source)
  {
    return new SqlServerQueryBuilder(source);
  }

  protected override Relational.AggregateRepository CreateRepository(IEventBus eventBus)
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    string connectionString = configuration.GetValue<string>("SQLCONNSTR_IntegrationTests")
      ?? throw new InvalidOperationException("The configuration 'SQLCONNSTR_IntegrationTests' has not been set.");

    Connection = new SqlConnection(connectionString);

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
