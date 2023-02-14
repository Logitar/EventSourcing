namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;

[Trait(Traits.Category, Categories.Unit)]
public class EventEntityTests
{
    [Theory]
    [InlineData("Test")]
    public void Given_aggregate_has_changes_Then_correct_events(string name)
    {
        TestAggregate aggregate = new();
        aggregate.Rename(name);
        aggregate.Delete();

        string aggregateType = aggregate.GetType().GetName();
        Dictionary<Guid, EventEntity> events = EventEntity.FromChanges(aggregate)
          .ToDictionary(x => x.Id, x => x);
        foreach (DomainEvent change in aggregate.Changes)
        {
            EventEntity entity = events[change.Id];

            string eventData = EventSerializer.Instance.Serialize(change);

            Assert.Equal(change.Version, entity.Version);
            Assert.Equal(change.ActorId, new AggregateId(entity.ActorId));
            Assert.Equal(change.OccurredOn, entity.OccurredOn);
            Assert.Equal(change.DeleteAction, entity.DeleteAction);
            Assert.Equal(aggregateType, entity.AggregateType);
            Assert.Equal(aggregate.Id, new AggregateId(entity.AggregateId));
            Assert.Equal(change.GetType().GetName(), entity.EventType);
            Assert.Equal(eventData, entity.EventData);
        }
    }

    [Fact]
    public void Given_aggregate_has_no_change_Then_no_events()
    {
        TestAggregate aggregate = new();
        Assert.Empty(EventEntity.FromChanges(aggregate));
    }
}
