using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Logitar.EventSourcing.MongoDB;

[Trait(Traits.Category, Categories.Integration)]
public class AggregateRepositoryTests : Infrastructure.AggregateRepositoryTests
{
  private const string CollectionName = "events";

  private IMongoDatabase? _mongoDatabase = null;
  private IMongoCollection<EventEntity>? _eventCollection = null;

  protected override void AssertEqual(IEventEntity _expected, IEventEntity _actual)
  {
    if (_expected is not EventEntity expected)
    {
      throw new ArgumentException($"The entity must be an instance of the {typeof(EventEntity)} class.", nameof(_expected));
    }
    if (_actual is not EventEntity actual)
    {
      throw new ArgumentException($"The entity must be an instance of the {typeof(EventEntity)} class.", nameof(_actual));
    }

    Assert.Equal(expected.EventId, actual.EventId);
    Assert.Equal(expected.Id, actual.Id);
    Assert.Equal(expected.ActorId, actual.ActorId);
    Assert.Equal(expected.IsDeleted, actual.IsDeleted);
    Assert.Equal(ToUnixTimeMilliseconds(expected.OccurredOn), ToUnixTimeMilliseconds(actual.OccurredOn));
    Assert.Equal(expected.Version, actual.Version);
    Assert.Equal(expected.AggregateType, actual.AggregateType);
    Assert.Equal(expected.AggregateId, actual.AggregateId);
    Assert.Equal(expected.EventType, actual.EventType);
    Assert.Equal(expected.EventData, actual.EventData);
  }
  private static long ToUnixTimeMilliseconds(DateTime moment) => new DateTimeOffset(moment).ToUnixTimeMilliseconds();

  protected override Infrastructure.AggregateRepository CreateRepository(IEventBus eventBus)
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    MongoDBSettings settings = configuration.GetSection("MongoDB").Get<MongoDBSettings>() ?? new();

    MongoClient client = new(settings.ConnectionString);
    _mongoDatabase = client.GetDatabase(settings.DatabaseName);
    _mongoDatabase.DropCollection(CollectionName);
    _eventCollection = _mongoDatabase.GetCollection<EventEntity>(CollectionName);

    return new AggregateRepository(eventBus, EventSerializer, _mongoDatabase);
  }

  protected override IEnumerable<IEventEntity> GetEventEntities(AggregateRoot aggregate)
  {
    return EventEntity.FromChanges(aggregate, EventSerializer);
  }

  protected override async Task<IEnumerable<IEventEntity>> LoadEventsAsync(CancellationToken cancellationToken = default)
  {
    return await _eventCollection.Find(Builders<EventEntity>.Filter.Empty).ToListAsync(cancellationToken);
  }

  protected override async Task SeedDatabaseAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken = default)
  {
    Assert.NotNull(_eventCollection);

    IEnumerable<EventEntity> events = aggregates.SelectMany(aggregate => EventEntity.FromChanges(aggregate, EventSerializer));
    await _eventCollection.InsertManyAsync(events, new InsertManyOptions(), cancellationToken);
  }
}
