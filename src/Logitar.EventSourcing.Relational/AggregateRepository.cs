using Logitar.Data;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Relational;

/// <summary>
/// Represents an interface that allows retrieving and storing events in a relational event store.
/// </summary>
public abstract class AggregateRepository : Infrastructure.AggregateRepository
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateRepository"/> class.
  /// </summary>
  /// <param name="connection">The database connection.</param>
  /// <param name="eventBus">The event bus.</param>
  /// <param name="eventSerializer">The serializer for events.</param>
  public AggregateRepository(DbConnection connection, IEventBus eventBus,
    IEventSerializer eventSerializer) : base(eventBus, eventSerializer)
  {
    Connection = connection;
  }

  /// <summary>
  /// Gets the database connection.
  /// </summary>
  protected DbConnection Connection { get; }

  /// <summary>
  /// Loads the events of an aggregate from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregate.</typeparam>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="version">The version at which the aggregate shall be retrieved.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded events.</returns>
  protected override async Task<IEnumerable<DomainEvent>> LoadChangesAsync<T>(AggregateId id, long? version, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(T).GetName();
    string aggregateId = id.Value;

    List<Condition> conditions = new(capacity: 3)
    {
      new OperatorCondition(Events.AggregateType, Operators.IsEqualTo(aggregateType)),
      new OperatorCondition(Events.AggregateId, Operators.IsEqualTo(aggregateId))
    };
    if (version.HasValue)
    {
      conditions.Add(new OperatorCondition(Events.Version, Operators.IsLessThanOrEqualTo(version.Value)));
    }

    IQuery query = From(Events.Table)
      .WhereAnd(conditions.ToArray())
      .OrderBy(Events.Version)
      .Select(Events.Id, Events.EventType, Events.EventData)
      .Build();

    return await ReadChangesAsync(query, cancellationToken);
  }

  /// <summary>
  /// Loads the events of all aggregates of the specified type from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded events.</returns>
  protected override async Task<IEnumerable<DomainEvent>> LoadChangesAsync<T>(CancellationToken cancellationToken)
  {
    string aggregateType = typeof(T).GetName();

    IQuery query = From(Events.Table)
      .Where(Events.AggregateType, Operators.IsEqualTo(aggregateType))
      .OrderBy(Events.Version)
      .Select(Events.Id, Events.EventType, Events.EventData)
      .Build();

    return await ReadChangesAsync(query, cancellationToken);
  }

  /// <summary>
  /// Loads the events of the specified aggregates from the event store.
  /// </summary>
  /// <typeparam name="T">The type of the aggregates.</typeparam>
  /// <param name="ids">The identifier of the aggregates.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of loaded events.</returns>
  protected override async Task<IEnumerable<DomainEvent>> LoadChangesAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(T).GetName();
    string[] aggregateIds = ids.Select(id => id.Value).Distinct().ToArray();

    IQuery query = From(Events.Table)
      .WhereAnd(new OperatorCondition(Events.AggregateType, Operators.IsEqualTo(aggregateType)),
        new OperatorCondition(Events.AggregateId, Operators.IsIn(aggregateIds)))
      .OrderBy(Events.Version)
      .Select(Events.Id, Events.EventType, Events.EventData)
      .Build();

    return await ReadChangesAsync(query, cancellationToken);
  }

  /// <summary>
  /// Reads changes from the specified data query.
  /// </summary>
  /// <param name="query">The data query.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded changes.</returns>
  protected virtual async Task<IEnumerable<DomainEvent>> ReadChangesAsync(IQuery query, CancellationToken cancellationToken)
  {
    using DbCommand command = Connection.CreateCommand();
    command.CommandText = query.Text;
    foreach (object parameter in query.Parameters)
    {
      command.Parameters.Add(parameter);
    }

    List<DomainEvent> changes = new();

    using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
    if (reader.HasRows)
    {
      while (await reader.ReadAsync(cancellationToken))
      {
        EventEntity entity = new()
        {
          Id = reader.GetGuid(0),
          EventType = reader.GetString(1),
          EventData = reader.GetString(2)
        };

        changes.Add(EventSerializer.Deserialize(entity));
      }
    }

    return changes.AsReadOnly();
  }

  /// <summary>
  /// Saves the changes of the specified aggregates to the event store.
  /// </summary>
  /// <param name="aggregates">The aggregates to save the changes.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  protected override async Task SaveChangesAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken)
  {
    IInsertBuilder builder = InsertInto(Events.Id, Events.ActorId, Events.IsDeleted,
      Events.OccurredOn, Events.Version, Events.AggregateType, Events.AggregateId, Events.EventType,
      Events.EventData);

    foreach (AggregateRoot aggregate in aggregates)
    {
      if (aggregate.HasChanges)
      {
        string aggregateType = aggregate.GetType().GetName();
        string aggregateId = aggregate.Id.Value;

        foreach (DomainEvent change in aggregate.Changes)
        {
          builder = builder.Value(change.Id, change.ActorId.Value, change.IsDeleted,
            change.OccurredOn.ToUniversalTime(), change.Version, aggregateType, aggregateId,
            change.GetType().GetName(), EventSerializer.Serialize(change));
        }
      }
    }

    ICommand insert = builder.Build();

    using DbCommand command = Connection.CreateCommand();
    command.CommandText = insert.Text;
    command.Parameters.AddRange(insert.Parameters.ToArray());
    await command.ExecuteNonQueryAsync(cancellationToken);
  }

  /// <summary>
  /// Returns a specific implementation of a query builder.
  /// </summary>
  /// <param name="source">The source table.</param>
  /// <returns>The query builder.</returns>
  protected abstract IQueryBuilder From(TableId source);
  /// <summary>
  /// Returns a specific implementation of an insert command builder.
  /// </summary>
  /// <param name="columns">The columns to insert into.</param>
  /// <returns>The insert command builder.</returns>
  protected abstract IInsertBuilder InsertInto(params ColumnId[] columns);
}
