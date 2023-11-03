using Logitar.Data;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Relational;

public abstract class AggregateRepositoryTests : Infrastructure.AggregateRepositoryTests, IDisposable
{
  protected AggregateRepositoryTests() : base()
  {
    Assert.NotNull(Connection);
    Connection.Open();
    RecreateDatabaseAsync().Wait();
  }

  public void Dispose()
  {
    Connection?.Dispose();
    GC.SuppressFinalize(this);
  }

  protected DbConnection? Connection { get; set; }

  protected override void AssertEqual(IEventEntity expected, IEventEntity actual)
  {
    Assert.Equal(expected.Id, actual.Id);
    Assert.Equal(expected.EventType, actual.EventType);
    Assert.Equal(expected.EventData, actual.EventData);
  }

  protected override IEnumerable<IEventEntity> GetEventEntities(AggregateRoot aggregate)
  {
    string aggregateType = aggregate.GetType().GetName();
    string aggregateId = aggregate.Id.Value;

    return aggregate.Changes.Select(change => new EventEntity
    {
      Id = change.Id,
      ActorId = change.ActorId.Value,
      IsDeleted = change.IsDeleted,
      OccurredOn = change.OccurredOn.ToUniversalTime(),
      Version = change.Version,
      AggregateType = aggregateType,
      AggregateId = aggregateId,
      EventType = change.GetType().GetName(),
      EventData = EventSerializer.Serialize(change)
    });
  }

  protected override async Task<IEnumerable<IEventEntity>> LoadEventsAsync(CancellationToken cancellationToken)
  {
    Assert.NotNull(Connection);

    IQuery query = CreateQueryBuilder(Events.Table)
      .Select(Events.Id, Events.EventType, Events.EventData)
      .Build();

    using DbCommand command = Connection.CreateCommand();
    command.CommandText = query.Text;
    command.Parameters.AddRange(query.Parameters.ToArray());

    List<EventEntity> entities = new();

    using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
    if (reader.HasRows)
    {
      while (await reader.ReadAsync(cancellationToken))
      {
        entities.Add(new EventEntity
        {
          Id = reader.GetGuid(0),
          EventType = reader.GetString(1),
          EventData = reader.GetString(2)
        });
      }
    }

    return entities;
  }

  protected override async Task SeedDatabaseAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken)
  {
    Assert.NotNull(Connection);

    ColumnId[] columns = new[] { Events.Id, Events.ActorId, Events.IsDeleted, Events.OccurredOn,
      Events.Version, Events.AggregateType, Events.AggregateId, Events.EventType, Events.EventData };
    IInsertBuilder builder = CreateInsertBuilder(columns);

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

  protected abstract IInsertBuilder CreateInsertBuilder(params ColumnId[] columns);
  protected abstract IQueryBuilder CreateQueryBuilder(TableId source);

  protected abstract override AggregateRepository CreateRepository(IEventBus eventBus);

  protected abstract Task RecreateDatabaseAsync(CancellationToken cancellationToken = default);
}
