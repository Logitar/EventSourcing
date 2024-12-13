using Bogus;

namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class AggregateRootTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "Apply: it should throw StreamMismatchException when applying an event from another stream.")]
  public void Given_EventFromAnotherStream_When_Apply_Then_StreamMismatchException()
  {
    User user = new();

    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1
    };

    var exception = Assert.Throws<StreamMismatchException>(() => user.LoadFromChanges(user.Id, [created]));
    Assert.Equal(user.Id.Value, exception.AggregateStreamId);
    Assert.Equal(created.StreamId.Value, exception.EventStreamId);
    Assert.Equal(created.Id.Value, exception.EventId);
  }

  [Fact(DisplayName = "Apply: it should throw UnexpectedEventVersionException when the event version was not expected.")]
  public void Given_UnexpectedEventVersion_When_Apply_Then_UnexpectedEventVersionException()
  {
    User user = new(_faker.Person.UserName);

    UserCreated? @event = Assert.Single(user.Changes) as UserCreated;
    Assert.NotNull(@event);

    var exception = Assert.Throws<UnexpectedEventVersionException>(() => user.LoadFromChanges(user.Id, [@event]));
    Assert.Equal(user.Id.Value, exception.AggregateId);
    Assert.Equal(user.Version, exception.AggregateVersion);
    Assert.Equal(@event.Id.Value, exception.EventId);
    Assert.Equal(@event.Version, exception.EventVersion);
  }

  [Fact(DisplayName = "ClearChanges: it should clear the changes.")]
  public void Given_Changes_When_ClearChanges_Then_ChangesAreCleared()
  {
    User user = new(_faker.Person.UserName);
    user.SignIn();
    user.Disable();
    user.Delete();

    Assert.True(user.HasChanges);
    Assert.NotEmpty(user.Changes);

    user.ClearChanges();
    Assert.False(user.HasChanges);
    Assert.Empty(user.Changes);

    user.ClearChanges();
  }

  [Theory(DisplayName = "ctor: it should assign the correct stream identifier.")]
  [InlineData(null)]
  [InlineData("test")]
  public void Given_StreamId_When_ctor_Then_CorrectStreamId(string? idValue)
  {
    if (idValue == null)
    {
      User user = new(id: null);
      Assert.False(string.IsNullOrWhiteSpace(user.Id.Value));
    }
    else
    {
      StreamId id = new(idValue);
      User user = new(id);
      Assert.Equal(id, user.Id);
    }
  }

  [Fact(DisplayName = "ctor: it should constuct an aggregate root without arguments.")]
  public void Given_NoArgument_When_ctor_Then_DefaultAggregate()
  {
    User user = new();
    Assert.False(string.IsNullOrWhiteSpace(user.Id.Value));
  }

  [Fact(DisplayName = "ctor: it should throw ArgumentException when the stream identifier is empty.")]
  public void Given_EmptyStreamId_When_ctor_Then_ArgumentException()
  {
    var exception = Assert.Throws<ArgumentException>(() => new User(new StreamId()));
    Assert.StartsWith("The identifier value is required.", exception.Message);
    Assert.Equal("id", exception.ParamName);
  }

  [Fact(DisplayName = "Equals: it should return false when the aggregates have different identifiers.")]
  public void Given_DifferentAggregateIds_When_Equals_Then_False()
  {
    User user1 = new();
    User user2 = new();
    Assert.False(user1.Equals(user2));
  }

  [Fact(DisplayName = "Equals: it should return true when the aggregates are the same.")]
  public void Given_SameAggregates_When_Equals_Then_True()
  {
    User user1 = new();
    User user2 = new(user1.Id);
    Assert.True(user1.Equals(user1));
    Assert.True(user1.Equals(user2));
  }

  [Fact(DisplayName = "Equals: it should return true when the aggregates have different types but the same identifier.")]
  public void Given_DifferentAggregateTypes_When_Equals_Then_False()
  {
    User user = new();
    Session session = new(user.Id);
    Assert.True(user.Equals(session));
  }

  [Fact(DisplayName = "GetHashCode: it should return the correct hash code.")]
  public void Given_Aggregate_When_GetHashCode_Then_CorrectHashCode()
  {
    User user = new();
    Assert.Equal(user.Id.GetHashCode(), user.GetHashCode());
  }

  [Fact(DisplayName = "LoadFromChanges: it should assign the stream identifier and apply the changes.")]
  public void Given_IdAndChanges_When_LoadFromChanges_Then_AggregateLoaded()
  {
    StreamId id = StreamId.NewId();

    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = id,
      Version = 1
    };
    UserDeleted deleted = new()
    {
      StreamId = id,
      Version = 2
    };

    User user = new();
    user.LoadFromChanges(id, [created, deleted]);

    Assert.Equal(id, user.Id);
    Assert.Equal(2, user.Version);

    Assert.False(user.HasChanges);
    Assert.Empty(user.Changes);
  }

  [Fact(DisplayName = "Raise: it should apply a new change to the aggregate.")]
  public void Given_NewChange_When_Raise_Then_ChangeApplied()
  {
    User user = new(_faker.Person.UserName);

    ActorId actorId = new(user.Id.Value);
    user.SignIn(actorId);

    user.Delete(actorId);
    user.Disable(actorId);

    Assert.Equal(4, user.Version);
    Assert.Null(user.CreatedBy);
    Assert.Equal(actorId, user.UpdatedBy);
    Assert.True(user.IsDeleted);

    Assert.True(user.HasChanges);
    Assert.Equal(4, user.Changes.Count);
    Assert.Contains(user.Changes, e => e is UserCreated created && created.UniqueName == _faker.Person.UserName && created.StreamId == user.Id && created.Version == 1);
    Assert.Contains(user.Changes, e => e is UserSignedIn signedIn && signedIn.StreamId == user.Id && signedIn.Version == 2 && signedIn.ActorId == actorId);
    Assert.Contains(user.Changes, e => e is UserDeleted deleted && deleted.StreamId == user.Id && deleted.Version == 3 && deleted.ActorId == actorId);
    Assert.Contains(user.Changes, e => e is UserDisabled disabled && disabled.StreamId == user.Id && disabled.Version == 4 && disabled.ActorId == actorId);

    Assert.Equal(((DomainEvent)user.Changes.First()).OccurredOn, user.CreatedOn);
    Assert.Equal(((UserDisabled)user.Changes.Last()).OccurredOn, user.UpdatedOn);
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_Aggregate_When_ToString_Then_CorrectStringRepresentation()
  {
    User user = new();
    Assert.Equal(string.Format("Logitar.EventSourcing.User (Id={0})", user.Id), user.ToString());
  }
}
