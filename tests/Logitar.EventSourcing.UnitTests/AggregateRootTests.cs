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
    User user = new();

    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = user.Id,
      Version = 1
    };
    UserPasswordCreated password = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")))
    {
      StreamId = user.Id,
      Version = 2
    };
    UserSignedIn signedIn = new()
    {
      StreamId = user.Id,
      Version = 3
    };

    var exception = Assert.Throws<UnexpectedEventVersionException>(() => user.LoadFromChanges(user.Id, [created, signedIn, password]));
    Assert.Equal(user.Id.Value, exception.AggregateId);
    Assert.Equal(user.Version, exception.AggregateVersion);
    Assert.Equal(signedIn.Id.Value, exception.EventId);
    Assert.Equal(signedIn.Version, exception.EventVersion);
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

  [Fact(DisplayName = "LoadFromChanges: it should assign the stream identifier.")]
  public void Given_IdWithoutChanges_When_LoadFromChanges_Then_AggregateLoaded()
  {
    StreamId id = StreamId.NewId();

    User user = new();
    user.LoadFromChanges(id, []);

    Assert.Equal(id, user.Id);
    Assert.Equal(0, user.Version);
    Assert.Null(user.CreatedBy);
    Assert.Equal(default, user.CreatedOn);
    Assert.Null(user.UpdatedBy);
    Assert.Equal(default, user.UpdatedOn);
    Assert.False(user.IsDeleted);

    Assert.False(user.HasChanges);
    Assert.Empty(user.Changes);
  }

  [Fact(DisplayName = "LoadFromChanges: it should throw InvalidOperationException when the aggregate version is greater than 0.")]
  public void Given_VersionGreaterThan0_When_LoadFromChanges_Then_InvalidOperationException()
  {
    User user = new(_faker.Person.UserName);

    UserCreated? @event = Assert.Single(user.Changes) as UserCreated;
    Assert.NotNull(@event);

    var exception = Assert.Throws<InvalidOperationException>(() => user.LoadFromChanges(user.Id, [@event]));
    Assert.Equal("The aggregate cannot be loaded once changes have been applied to it.", exception.Message);
  }

  [Fact(DisplayName = "LoadFromSnapshot: it should assign the stream identifier and metadata, then apply the changes.")]
  public void Given_SnapshotAndChanges_When_LoadFromSnapshot_Then_AggregateLoaded()
  {
    User user = new();

    UserSnapshot snapshot = new()
    {
      UniqueName = _faker.Person.UserName,
      SignedInOn = DateTime.Now.AddMinutes(-1),
      Version = 3,
      CreatedOn = DateTime.Now.AddHours(-1),
      UpdatedBy = new ActorId(user.Id.Value),
      UpdatedOn = DateTime.Now.AddMinutes(-1),
      IsDeleted = true
    };

    UserDisabled disabled = new(user.Id, version: 4, actorId: snapshot.UpdatedBy);

    user.LoadFromSnapshot(user.Id, snapshot, [disabled]);

    Assert.Equal(disabled.StreamId, user.Id);
    Assert.Equal(disabled.Version, user.Version);
    Assert.Equal(snapshot.CreatedBy, user.CreatedBy);
    Assert.Equal(snapshot.CreatedOn, user.CreatedOn);
    Assert.Equal(disabled.ActorId, user.UpdatedBy);
    Assert.Equal(disabled.OccurredOn, user.UpdatedOn);
    Assert.Equal(snapshot.IsDeleted, user.IsDeleted);

    Assert.Equal(snapshot.UniqueName, user.UniqueName);
    Assert.True(user.IsDisabled);
    Assert.Equal(snapshot.SignedInOn, user.SignedInOn);

    Assert.False(user.HasChanges);
    Assert.Empty(user.Changes);
  }

  [Fact(DisplayName = "LoadFromSnapshot: it should assign the stream identifier and metadata.")]
  public void Given_SnapshotWithoutChanges_When_LoadFromSnapshot_Then_AggregateLoaded()
  {
    User user = new();
    StreamId userId = StreamId.NewId();

    UserSnapshot snapshot = new()
    {
      UniqueName = _faker.Person.UserName,
      SignedInOn = DateTime.Now.AddMinutes(-1),
      Version = 3,
      CreatedOn = DateTime.Now.AddHours(-1),
      UpdatedBy = new ActorId(user.Id.Value),
      UpdatedOn = DateTime.Now.AddMinutes(-1),
      IsDeleted = true
    };

    user.LoadFromSnapshot(userId, snapshot);

    Assert.Equal(userId, user.Id);
    Assert.Equal(snapshot.Version, user.Version);
    Assert.Equal(snapshot.CreatedBy, user.CreatedBy);
    Assert.Equal(snapshot.CreatedOn, user.CreatedOn);
    Assert.Equal(snapshot.UpdatedBy, user.UpdatedBy);
    Assert.Equal(snapshot.UpdatedOn, user.UpdatedOn);
    Assert.Equal(snapshot.IsDeleted, user.IsDeleted);

    Assert.Equal(snapshot.UniqueName, user.UniqueName);
    Assert.Equal(snapshot.IsDisabled, user.IsDisabled);
    Assert.Equal(snapshot.SignedInOn, user.SignedInOn);

    Assert.False(user.HasChanges);
    Assert.Empty(user.Changes);
  }

  [Theory(DisplayName = "LoadFromSnapshot: it should throw ArgumentOutOfRangeException when the snapshot version is not greater than 0.")]
  [InlineData(0)]
  [InlineData(-1)]
  public void Given_SnapshotVersionNotGreaterThan0_When_LoadFromSnapshot_Then_ArgumentOutOfRangeException(long version)
  {
    User user = new();

    AggregateSnapshot snapshot = new()
    {
      Version = version
    };
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => user.LoadFromSnapshot(user.Id, snapshot));
    Assert.StartsWith("The version must be greater than 0.", exception.Message);
    Assert.Equal("snapshot", exception.ParamName);
  }

  [Fact(DisplayName = "LoadFromSnapshot: it should throw InvalidOperationException when the aggregate version is greater than 0.")]
  public void Given_VersionGreaterThan0_When_LoadFromSnapshot_Then_InvalidOperationException()
  {
    User user = new(_faker.Person.UserName);

    UserCreated? @event = Assert.Single(user.Changes) as UserCreated;
    Assert.NotNull(@event);

    AggregateSnapshot snapshot = new()
    {
      Version = user.Version,
      CreatedBy = user.CreatedBy,
      CreatedOn = user.CreatedOn,
      UpdatedBy = user.UpdatedBy,
      UpdatedOn = user.UpdatedOn,
      IsDeleted = false
    };
    var exception = Assert.Throws<InvalidOperationException>(() => user.LoadFromSnapshot(user.Id, snapshot));
    Assert.Equal("The aggregate cannot be loaded once changes have been applied to it.", exception.Message);
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
