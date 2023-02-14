namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class EventNotSupportedExceptionTests
{
  [Fact]
  public void When_constructed_Then_it_has_correct_data()
  {
    Type aggregateType = typeof(TestAggregate);
    Type eventType = typeof(TestEvent);

    EventNotSupportedException exception = new(aggregateType, eventType);

    Assert.Equal(aggregateType.GetName(), exception.Data["AggregateType"]);
    Assert.Equal(eventType.GetName(), exception.Data["EventType"]);
  }
}
