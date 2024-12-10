namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class DomainEventTests
{
  [Fact(DisplayName = "ctor: it should construct the domain event correctly.")]
  public void Given_DomainEvent_When_ctor_Then_ConstructedCorrectly()
  {
    UserSignedIn @event = new();

    Assert.False(string.IsNullOrWhiteSpace(@event.Id.Value));
    Assert.True(string.IsNullOrWhiteSpace(@event.StreamId.Value));
    Assert.Equal(0, @event.Version);
    Assert.Null(@event.ActorId);
    Assert.Equal(DateTime.Now, @event.OccurredOn, TimeSpan.FromSeconds(1));
    Assert.Null(@event.IsDeleted);
  }
}
