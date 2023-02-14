namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class DomainEventTests
{
  [Fact]
  public void When_constructed_Then_it_has_default_values()
  {
    TestEvent e = new();

    Assert.NotEqual(Guid.Empty, e.Id);

    Assert.Equal(default, e.AggregateId);
    Assert.Equal(0, e.Version);

    Assert.Equal("SYSTEM", e.ActorId.Value);
    Assert.NotEqual(default, e.OccurredOn);

    Assert.Equal(DeleteAction.None, e.DeleteAction);
  }
}
