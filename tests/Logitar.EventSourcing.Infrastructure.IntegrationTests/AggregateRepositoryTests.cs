using Bogus;
using Moq;

namespace Logitar.EventSourcing.Infrastructure;

public abstract class AggregateRepositoryTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEventBus> _eventBus = new();

  private readonly PersonAggregate _person1;
  private readonly PersonAggregate _person2;
  private readonly PersonAggregate _person3;
  private readonly PersonAggregate _deleted;
  private readonly ContactAggregate _contact;

  private readonly AggregateRepository _repository;

  protected AggregateRepositoryTests()
  {
    _person1 = new(new Faker().Person.FullName);
    _person2 = new(new Faker().Person.FullName);
    _person3 = new(new Faker().Person.FullName);

    Faker faker = new();
    _deleted = new(faker.Person.FullName);
    _deleted.Delete();
    _contact = new(_deleted, ContactType.Email, faker.Person.Email);

    _repository = CreateRepository(_eventBus.Object);
  }

  protected IEventSerializer EventSerializer { get; } = new EventSerializer();

  private IEnumerable<AggregateRoot> Aggregates => new AggregateRoot[] { _person1, _person2, _person3, _deleted, _contact };

  [Theory(DisplayName = "It should load the correct aggregate by id.")]
  [InlineData(false)]
  [InlineData(true)]
  [InlineData(false, 1)]
  [InlineData(true, 1)]
  public async Task It_should_load_the_correct_aggregate_by_id(bool includeDeleted, int? version = null)
  {
    await SeedDatabaseAsync(Aggregates);

    PersonAggregate? person = await _repository.LoadAsync<PersonAggregate>(_deleted.Id, version, includeDeleted, _cancellationToken);
    Assert.Equal((includeDeleted || version.HasValue) ? _deleted : null, person);

    if (person != null && version.HasValue)
    {
      Assert.Equal(version.Value, person.Version);
    }
  }

  [Theory(DisplayName = "It should load the correct aggregates by ids.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task It_should_load_the_correct_aggregates_by_ids(bool includeDeleted)
  {
    await SeedDatabaseAsync(Aggregates);

    AggregateId[] ids = new[] { _person1.Id, _person2.Id, _deleted.Id, _contact.Id };
    IEnumerable<PersonAggregate> people = await _repository.LoadAsync<PersonAggregate>(ids, includeDeleted, _cancellationToken);
    Assert.Equal(includeDeleted ? 3 : 2, people.Count());

    Assert.Contains(_person1, people);
    Assert.Contains(_person2, people);

    if (includeDeleted)
    {
      Assert.Contains(_deleted, people);
    }
  }

  [Theory(DisplayName = "It should load the correct typed aggregates.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task It_should_load_the_correct_typed_aggregates(bool includeDeleted)
  {
    await SeedDatabaseAsync(Aggregates);

    IEnumerable<PersonAggregate> people = await _repository.LoadAsync<PersonAggregate>(includeDeleted, _cancellationToken);
    Assert.Equal(includeDeleted ? 4 : 3, people.Count());

    Assert.Contains(_person1, people);
    Assert.Contains(_person2, people);
    Assert.Contains(_person3, people);

    if (includeDeleted)
    {
      Assert.Contains(_deleted, people);
    }
  }

  [Fact(DisplayName = "It should persist and publish the correct events from aggregates.")]
  public async Task It_should_persist_and_publish_the_correct_events_from_aggregates()
  {
    await AssertDatabaseIsEmptyAsync();

    PersonAggregate[] aggregates = new[] { _person1, _person2, _person3 };
    DomainEvent[] changes = aggregates.SelectMany(p => p.Changes).ToArray();
    Dictionary<Guid, IEventEntity> events = aggregates.SelectMany(GetEventEntities)
      .ToDictionary(e => e.Id, e => e);

    await _repository.SaveAsync(aggregates, _cancellationToken);

    IEnumerable<IEventEntity> entities = await LoadEventsAsync();
    Assert.Equal(events.Count, entities.Count());
    foreach (IEventEntity entity in entities)
    {
      AssertEqual(events[entity.Id], entity);
    }

    _eventBus.Verify(x => x.PublishAsync(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()),
      Times.Exactly(changes.Length));
    foreach (DomainEvent change in changes)
    {
      _eventBus.Verify(x => x.PublishAsync(change, _cancellationToken), Times.Once);
    }
  }

  [Fact(DisplayName = "It should persist and publish the correct events from an aggregate.")]
  public async Task It_should_persist_and_publish_the_correct_events_from_an_aggregate()
  {
    await AssertDatabaseIsEmptyAsync();

    DomainEvent[] changes = _deleted.Changes.ToArray();
    Dictionary<Guid, IEventEntity> events = GetEventEntities(_deleted)
      .ToDictionary(e => e.Id, e => e);

    await _repository.SaveAsync(_deleted, _cancellationToken);

    IEnumerable<IEventEntity> entities = await LoadEventsAsync();
    Assert.Equal(events.Count, entities.Count());
    foreach (IEventEntity entity in entities)
    {
      AssertEqual(events[entity.Id], entity);
    }

    _eventBus.Verify(x => x.PublishAsync(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()),
      Times.Exactly(changes.Length));
    foreach (DomainEvent change in changes)
    {
      _eventBus.Verify(x => x.PublishAsync(change, _cancellationToken), Times.Once);
    }
  }

  protected abstract void AssertEqual(IEventEntity expected, IEventEntity actual);

  protected abstract AggregateRepository CreateRepository(IEventBus eventBus);

  protected abstract IEnumerable<IEventEntity> GetEventEntities(AggregateRoot aggregate);

  protected abstract Task<IEnumerable<IEventEntity>> LoadEventsAsync(CancellationToken cancellationToken = default);

  protected abstract Task SeedDatabaseAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken = default);

  private async Task AssertDatabaseIsEmptyAsync(CancellationToken cancellationToken = default)
  {
    IEnumerable<IEventEntity> events = await LoadEventsAsync(cancellationToken);
    Assert.Empty(events);
  }
}
