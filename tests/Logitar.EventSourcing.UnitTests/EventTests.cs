using Bogus;

namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class EventTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "ctor: it should construct the event from arguments.")]
  public void Given_Arguments_When_ctor_Then_EventIsConstructed()
  {
    EventId id = EventId.NewId();
    long version = 1;
    DateTime occurredOn = DateTime.Now;
    UserCreated data = new(_faker.Person.UserName);
    ActorId actorId = new(_faker.Internet.UserName());
    bool isDeleted = false;

    Event @event = new(id, version, occurredOn, data, actorId, isDeleted);

    Assert.Equal(id, @event.Id);
    Assert.Equal(version, @event.Version);
    Assert.Equal(occurredOn, @event.OccurredOn);
    Assert.Same(data, @event.Data);
    Assert.Equal(actorId, @event.ActorId);
    Assert.Equal(isDeleted, @event.IsDeleted);
  }

  [Fact(DisplayName = "Equals: it should return false when comparing different events.")]
  public void Given_DifferentEvents_When_Equals_Then_False()
  {
    Event event1 = new(EventId.NewId(), version: 1, DateTime.Now, new UserCreated(_faker.Person.UserName));
    Event event2 = new(EventId.NewId(), version: 2, DateTime.Now, new UserSignedIn());
    Assert.False(event1.Equals(event2));
  }

  [Fact(DisplayName = "Equals: it should return false when comparing with an object of a different type.")]
  public void Given_DifferentTypes_When_Equals_Then_False()
  {
    Event @event = new(EventId.NewId(), version: 1, DateTime.Now, new UserCreated(_faker.Person.UserName));
    User user = new();
    Assert.False(@event.Equals(user));
  }

  [Fact(DisplayName = "Equals: it should return true when comparing the same event.")]
  public void Given_SameEvent_When_Equals_Then_True()
  {
    Event event1 = new(EventId.NewId(), version: 1, DateTime.Now, new UserCreated(_faker.Person.UserName));
    Event event2 = new(event1.Id, version: 2, DateTime.Now, new UserSignedIn());
    Assert.True(event1.Equals(event1));
    Assert.True(event1.Equals(event2));
  }

  [Fact(DisplayName = "GetHashCode: it should return the correct hash code.")]
  public void Given_Event_When_GetHashCode_Then_CorrectHashCode()
  {
    Event @event = new(EventId.NewId(), version: 1, DateTime.Now, new UserCreated(_faker.Person.UserName));
    Assert.Equal(@event.Id.GetHashCode(), @event.GetHashCode());
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Event_When_ToString_Then_CorrectStringRepresentation()
  {
    Event @event = new(EventId.NewId(), version: 1, DateTime.Now, new UserCreated(_faker.Person.UserName));
    Assert.Equal(string.Format("Logitar.EventSourcing.UserCreated | Logitar.EventSourcing.Event (Id={0})", @event.Id), @event.ToString());
  }
}
