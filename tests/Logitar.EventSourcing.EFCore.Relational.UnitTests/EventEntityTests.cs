using Bogus;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

[Trait(Traits.Category, Categories.Unit)]
public class EventEntityTests
{
  private readonly EventSerializer _eventSerializer = new();
  private readonly Faker _faker = new();

  [Fact(DisplayName = "FromChanges: it should return the correct changes.")]
  public void FromChanges_it_should_return_the_correct_changes()
  {
    PersonAggregate person = new(_faker.Person.FullName);
    person.Delete();

    PersonCreatedEvent created = (PersonCreatedEvent)person.Changes.First();
    PersonDeletedChangedEvent deleted = (PersonDeletedChangedEvent)person.Changes.Skip(1).Single();

    EventEntity[] events = EventEntity.FromChanges(person, _eventSerializer).ToArray();
    AssertEqual(events[0], created, person);
    AssertEqual(events[1], deleted, person);
  }

  private void AssertEqual(EventEntity entity, DomainEvent change, AggregateRoot aggregate)
  {
    Assert.Equal(entity.Id, change.Id);
    Assert.Equal(entity.ActorId, change.ActorId.Value);
    Assert.Equal(entity.IsDeleted, change.IsDeleted);
    Assert.Equal(entity.OccurredOn, change.OccurredOn.ToUniversalTime());
    Assert.Equal(entity.Version, change.Version);
    Assert.Equal(entity.AggregateType, aggregate.GetType().GetName());
    Assert.Equal(entity.AggregateId, aggregate.Id.Value);
    Assert.Equal(entity.EventType, change.GetType().GetName());
    Assert.Equal(entity.EventData, _eventSerializer.Serialize(change));
  }
}
