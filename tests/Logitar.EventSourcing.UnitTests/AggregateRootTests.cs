namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class AggregateRootTests
{
  [Theory]
  [InlineData("Test")]
  public void Given_aggregate_has_changes_When_clear_changes_Then_changes_are_cleared(string name)
  {
    TestAggregate aggregate = new();

    aggregate.Rename(name);
    Assert.True(aggregate.HasChanges);
    Assert.NotEmpty(aggregate.Changes);

    aggregate.ClearChanges();
    Assert.False(aggregate.HasChanges);
    Assert.Empty(aggregate.Changes);
  }

  [Fact]
  public void Given_aggregate_ID_not_supplied_Then_it_has_random_ID()
  {
    TestAggregate aggregate = new();
    Assert.NotEqual(default, aggregate.Id);
  }

  [Fact]
  public void Given_aggregate_root_Then_correct_hash_code()
  {
    TestAggregate aggregate = new();
    int expected = HashCode.Combine(typeof(TestAggregate), aggregate.Id);
    Assert.Equal(expected, aggregate.GetHashCode());
  }

  [Fact]
  public void Given_aggregate_root_Then_correct_string_representation()
  {
    TestAggregate aggregate = new();
    string expected = string.Format("{0} (Id={1})", typeof(TestAggregate), aggregate.Id);
    Assert.Equal(expected, aggregate.ToString());
  }

  [Theory]
  [InlineData("Test")]
  public void Given_change_is_applied_Then_aggregate_has_changes(string name)
  {
    TestAggregate aggregate = new();
    Assert.Equal(0, aggregate.Version);
    Assert.False(aggregate.HasChanges);
    Assert.Empty(aggregate.Changes);

    aggregate.Rename(name);
    Assert.Equal(1, aggregate.Version);
    Assert.True(aggregate.HasChanges);
    Assert.NotEmpty(aggregate.Changes);
  }

  [Theory]
  [InlineData("Test")]
  public void Given_change_is_applied_Then_Apply_method_invoked(string name)
  {
    TestAggregate aggregate = new();
    aggregate.Rename(name);
    Assert.Equal(name, aggregate.Name);
  }

  [Theory]
  [InlineData("Test")]
  public void Given_change_is_applied_Then_event_is_updated(string name)
  {
    TestAggregate aggregate = new();
    aggregate.Rename(name);

    DomainEvent change = aggregate.Changes.Single();
    Assert.Equal(aggregate.Id, change.AggregateId);
    Assert.Equal(aggregate.Version, change.Version);
  }

  [Theory]
  [InlineData("Test")]
  public void Given_changes_are_applied_Then_aggregate_metadata_is_updated(string name)
  {
    TestAggregate aggregate = new();

    aggregate.Rename(name);
    DomainEvent change = aggregate.Changes.Single();
    Assert.Equal(change.ActorId, aggregate.CreatedById);
    Assert.Equal(change.ActorId, aggregate.UpdatedById);
    Assert.Equal(change.OccurredOn, aggregate.CreatedOn);
    Assert.Equal(change.OccurredOn, aggregate.UpdatedOn);

    aggregate.Rename(name);
    DomainEvent other = aggregate.Changes.Skip(1).Single();
    Assert.Equal(change.ActorId, aggregate.CreatedById);
    Assert.Equal(other.ActorId, aggregate.UpdatedById);
    Assert.Equal(change.OccurredOn, aggregate.CreatedOn);
    Assert.Equal(other.OccurredOn, aggregate.UpdatedOn);
  }

  [Theory]
  [InlineData("10027931-A0E0-4BD9-94AB-43872B20829E")]
  public void Given_constructed_with_ID_Then_same_ID(string id)
  {
    AggregateId aggregateId = new(id);
    TestAggregate aggregate = new(aggregateId);
    Assert.Equal(aggregateId, aggregate.Id);
  }

  [Fact]
  public void Given_different_type_Then_not_equal()
  {
    TestAggregate aggregate = new();
    OtherAggregate other = new();
    Assert.False(aggregate.Equals(other));
  }

  [Fact]
  public void Given_event_has_no_Apply_method_Then_EventNotSupportedException_is_thrown()
  {
    TestAggregate aggregate = new();
    EventNotSupportedException exception = Assert.Throws<EventNotSupportedException>(aggregate.Fail);

    Assert.Equal(aggregate.GetType().GetName(), exception.Data["AggregateType"]);
    Assert.Equal(typeof(AggregateFailed).GetName(), exception.Data["EventType"]);
  }

  [Fact]
  public void Given_has_been_deleted_Then_it_is_deleted()
  {
    TestAggregate aggregate = new();
    aggregate.Delete();
    Assert.NotNull(aggregate.DeletedById);
    Assert.NotNull(aggregate.DeletedOn);
    Assert.True(aggregate.IsDeleted);
  }

  [Fact]
  public void Given_has_not_been_deleted_Then_it_is_not_deleted()
  {
    TestAggregate aggregate = new();
    Assert.Null(aggregate.DeletedById);
    Assert.Null(aggregate.DeletedOn);
    Assert.False(aggregate.IsDeleted);
  }

  [Fact]
  public void Given_is_undeleted_Then_it_is_not_deleted()
  {
    TestAggregate aggregate = new();
    aggregate.Delete();
    aggregate.Undelete();
    Assert.False(aggregate.IsDeleted);
  }

  [Fact]
  public void Given_not_an_aggregate_root_Then_not_equal()
  {
    TestAggregate aggregate = new();
    object other = new();
    Assert.False(aggregate.Equals(other));
  }

  [Fact]
  public void Given_other_aggregate_event_in_history_Then_EventAggregateMismatchException_is_thrown()
  {
    TestAggregate aggregate = new();

    AggregateDeleted valid = new()
    {
      AggregateId = aggregate.Id,
      Version = 1
    };
    AggregateUndeleted invalid = new()
    {
      AggregateId = AggregateId.NewId(),
      Version = 2
    };

    EventAggregateMismatchException exception = Assert.Throws<EventAggregateMismatchException>(()
      => aggregate.LoadFromChanges(new DomainEvent[] { valid, invalid }));
    Assert.Equal(aggregate.ToString(), exception.Data["Aggregate"]);
    Assert.Equal(aggregate.Id.ToString(), exception.Data["AggregateId"]);
    Assert.Equal(invalid.ToString(), exception.Data["Event"]);
    Assert.Equal(invalid.Id, exception.Data["EventId"]);
    Assert.Equal(invalid.AggregateId.ToString(), exception.Data["EventAggregateId"]);
  }

  [Theory]
  [InlineData("Test")]
  public void Given_past_event_in_history_Then_CannotApplyPastEventException_is_thrown(string name)
  {
    TestAggregate aggregate = new();

    AggregateRenamed first = new(name)
    {
      AggregateId = aggregate.Id,
      Version = 1
    };
    DomainEvent[] changes = new DomainEvent[]
    {
      first,
      new AggregateDeleted
      {
        AggregateId = aggregate.Id,
        Version = 2
      },
      new AggregateUndeleted
      {
        AggregateId = aggregate.Id,
        Version = 3
      }
    };
    aggregate.LoadFromChanges(changes);

    CannotApplyPastEventException exception = Assert.Throws<CannotApplyPastEventException>(()
      => aggregate.LoadFromChanges(new[] { first }));
    Assert.Equal(aggregate.ToString(), exception.Data["Aggregate"]);
    Assert.Equal(aggregate.Id.ToString(), exception.Data["AggregateId"]);
    Assert.Equal(aggregate.Version, exception.Data["AggregateVersion"]);
    Assert.Equal(first.ToString(), exception.Data["Event"]);
    Assert.Equal(first.Id, exception.Data["EventId"]);
    Assert.Equal(first.Version, exception.Data["EventVersion"]);
  }

  [Fact]
  public void Given_same_type_and_ID_Then_equal()
  {
    TestAggregate aggregate = new();
    TestAggregate other = new(aggregate.Id);
    Assert.True(aggregate.Equals(other));
  }

  [Theory]
  [InlineData("Test")]
  public void Given_valid_history_Then_it_is_loaded(string name)
  {
    AggregateId id = AggregateId.NewId();

    AggregateRenamed first = new(name)
    {
      AggregateId = id,
      Version = 1
    };
    AggregateUndeleted last = new()
    {
      AggregateId = id,
      Version = 3
    };
    DomainEvent[] changes = new DomainEvent[]
    {
      first,
      last,
      new AggregateDeleted
      {
        AggregateId = id,
        Version = 2
      }
    };

    TestAggregate aggregate = new(id);
    aggregate.LoadFromChanges(changes);

    Assert.Equal(last.Version, aggregate.Version);

    Assert.Equal(first.ActorId, aggregate.CreatedById);
    Assert.Equal(first.OccurredOn, aggregate.CreatedOn);

    Assert.Equal(last.ActorId, aggregate.UpdatedById);
    Assert.Equal(last.OccurredOn, aggregate.UpdatedOn);

    Assert.False(aggregate.IsDeleted);
  }
}
