using Bogus;
using Moq;

namespace Logitar.EventSourcing.Infrastructure;

[Trait(Traits.Category, Categories.Unit)]
public class AggregateRepositoryTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEventBus> _eventBus = new();
  private readonly EventSerializer _eventSerializer = new();

  private readonly AggregateRepositoryMock _repository;

  public AggregateRepositoryTests()
  {
    _repository = new(_eventBus.Object, _eventSerializer);
  }

  [Fact(DisplayName = "LoadAsync: it should load the correct aggregates from events.")]
  public async Task LoadAsync_it_should_load_the_correct_aggregates_from_events()
  {
    PersonAggregate person1 = new(new Faker().Person.FullName);
    PersonAggregate person2 = new(new Faker().Person.FullName);
    PersonAggregate deleted = new(new Faker().Person.FullName);

    deleted.Delete();

    IEnumerable<DomainEvent> events = new[] { person1, person2, deleted }.SelectMany(p => p.Changes);
    _repository.Events.AddRange(events);

    AggregateId[] ids = new[] { person1.Id, person2.Id, deleted.Id };

    IEnumerable<PersonAggregate> people = await _repository.LoadAsync<PersonAggregate>(ids, includeDeleted: false, _cancellationToken);
    Assert.Equal(new[] { person1, person2 }, people);
  }

  [Fact(DisplayName = "SaveAsync: it should publish and clear aggregates changes.")]
  public async Task SaveAsync_it_should_publish_and_clear_aggregates_changes()
  {
    PersonAggregate person = new(new Faker().Person.FullName);
    PersonAggregate deleted = new(new Faker().Person.FullName);
    PersonAggregate noChange = new(AggregateId.NewId());

    deleted.Delete();

    Assert.True(person.HasChanges);
    Assert.True(deleted.HasChanges);
    Assert.False(noChange.HasChanges);

    await _repository.SaveAsync(new[] { person, deleted, noChange }, _cancellationToken);

    foreach (DomainEvent change in person.Changes)
    {
      _eventBus.Verify(x => x.PublishAsync(change, _cancellationToken), Times.Once);
    }
    foreach (DomainEvent change in deleted.Changes)
    {
      _eventBus.Verify(x => x.PublishAsync(change, _cancellationToken), Times.Once);
    }
    _eventBus.Verify(x => x.PublishAsync(
      It.Is<DomainEvent>(y => y.AggregateId != person.Id && y.AggregateId != deleted.Id), _cancellationToken),
      Times.Never);

    Assert.False(person.HasChanges);
    Assert.False(person.HasChanges);
    Assert.False(person.HasChanges);
  }
}
