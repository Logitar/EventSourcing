using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.SqlServer;

/// <summary>
/// Represents the Microsoft SQL Server implementation of an interface that allows retrieving and storing events in an event store.
/// </summary>
public class AggregateRepository : Relational.AggregateRepository
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateRepository"/> class.
  /// </summary>
  /// <param name="connection">The database connection.</param>
  /// <param name="eventBus">The event bus.</param>
  /// <param name="eventSerializer">The serializer for events.</param>
  public AggregateRepository(DbConnection connection, IEventBus eventBus,
    IEventSerializer eventSerializer) : base(connection, eventBus, eventSerializer)
  {
  }

  /// <summary>
  /// Returns the SQL Server implementation of a query builder.
  /// </summary>
  /// <param name="source">The source table.</param>
  /// <returns>The query builder.</returns>
  protected override IQueryBuilder From(TableId source) => new SqlServerQueryBuilder(source);
  /// <summary>
  /// Returns the SQL Server implementation of an insert command builder.
  /// </summary>
  /// <param name="columns">The columns to insert into.</param>
  /// <returns>The insert command builder.</returns>
  protected override IInsertBuilder InsertInto(params ColumnId[] columns) => new SqlServerInsertBuilder(columns);
}
