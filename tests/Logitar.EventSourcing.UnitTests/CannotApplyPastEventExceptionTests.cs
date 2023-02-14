namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class CannotApplyPastEventExceptionTests
{
  [Fact]
  public void When_constructed_Then_it_has_correct_data()
  {
    TestAggregate aggregate = new();
    TestEvent change = new()
    {
      AggregateId = AggregateId.NewId(),
      Version = -1
    };

    CannotApplyPastEventException exception = new(aggregate, change);
    Assert.Equal(aggregate.ToString(), exception.Data["Aggregate"]);
    Assert.Equal(aggregate.Id.ToString(), exception.Data["AggregateId"]);
    Assert.Equal(aggregate.Version, exception.Data["AggregateVersion"]);
    Assert.Equal(change.ToString(), exception.Data["Event"]);
    Assert.Equal(change.Id, exception.Data["EventId"]);
    Assert.Equal(change.Version, exception.Data["EventVersion"]);
  }
}
