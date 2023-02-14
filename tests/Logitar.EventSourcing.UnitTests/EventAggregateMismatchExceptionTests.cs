namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class EventAggregateMismatchExceptionTests
{
  [Fact]
  public void When_constructed_Then_it_has_correct_data()
  {
    TestAggregate aggregate = new();
    TestEvent change = new()
    {
      AggregateId = AggregateId.NewId()
    };

    EventAggregateMismatchException exception = new(aggregate, change);
    Assert.Equal(aggregate.ToString(), exception.Data["Aggregate"]);
    Assert.Equal(aggregate.Id.ToString(), exception.Data["AggregateId"]);
    Assert.Equal(change.ToString(), exception.Data["Event"]);
    Assert.Equal(change.Id, exception.Data["EventId"]);
    Assert.Equal(change.AggregateId.ToString(), exception.Data["EventAggregateId"]);
  }
}
