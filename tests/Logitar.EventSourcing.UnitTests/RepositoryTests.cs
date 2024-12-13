using Bogus;
using Moq;

namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class RepositoryTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IEventStore> _eventStore = new();

  private readonly Repository _repository;

  public RepositoryTests()
  {
    _repository = new Repository(_eventStore.Object);
  }

  [Theory(DisplayName = "LoadAsync: it should only return the matching aggregates.")]
  [InlineData(null)]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_IdsAndDeletionFlag_When_LoadAsync_Then_OnlyReturnsMatchingAggregates(bool? isDeleted)
  {
    User user1 = new(_faker.Person.UserName);
    if (isDeleted == true)
    {
      user1.Delete();
    }
    User user2 = new(_faker.Person.UserName);
    if (isDeleted != false)
    {
      user2.Delete();
    }

    Stream stream1 = new(user1.Id, user1.GetType(), user1.Changes.Select(ToEvent));
    Stream stream2 = new(user2.Id, user2.GetType(), user2.Changes.Select(ToEvent));
    _eventStore.Setup(x => x.FetchAsync(
      It.Is<FetchManyOptions>(y => y.StreamTypes.Count == 1 && y.StreamTypes.Contains(typeof(User)) && y.IsDeleted == isDeleted),
      _cancellationToken)).ReturnsAsync([stream1, stream2]);

    StreamId[] ids = [user1.Id, user2.Id, StreamId.NewId(), new StreamId()];
    IReadOnlyCollection<User> users = await _repository.LoadAsync<User>(ids, isDeleted, _cancellationToken);

    Assert.Equal(2, users.Count);
    Assert.Contains(user1, users);
    Assert.Contains(user2, users);
  }

  [Theory(DisplayName = "LoadAsync: it should return all aggregates of the same type, enforcing the deletion filter.")]
  [InlineData(null)]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_AllAggregatesSameType_When_LoadAsync_Then_AggregatesAreReturned(bool? isDeleted)
  {
    User user1 = new(_faker.Person.UserName);
    if (isDeleted == true)
    {
      user1.Delete();
    }
    User user2 = new(_faker.Person.UserName);
    if (isDeleted != false)
    {
      user2.Delete();
    }

    Stream stream1 = new(user1.Id, user1.GetType(), user1.Changes.Select(ToEvent));
    Stream stream2 = new(user2.Id, user2.GetType(), user2.Changes.Select(ToEvent));
    _eventStore.Setup(x => x.FetchAsync(
      It.Is<FetchManyOptions>(y => y.StreamTypes.Count == 1 && y.StreamTypes.Contains(typeof(User)) && y.IsDeleted == isDeleted),
      _cancellationToken)).ReturnsAsync([stream1, stream2]);

    IReadOnlyCollection<User> users = await _repository.LoadAsync<User>(isDeleted, _cancellationToken);

    Assert.Equal(2, users.Count);
    Assert.Contains(user1, users);
    Assert.Contains(user2, users);
  }

  [Fact(DisplayName = "LoadAsync: it should return empty when no aggregate was found.")]
  public async Task Given_NoAggregateFound_When_LoadAsync_Then_Empty()
  {
    _eventStore.Setup(x => x.FetchAsync(It.IsAny<FetchManyOptions>(), _cancellationToken)).ReturnsAsync([]);

    Assert.Empty(await _repository.LoadAsync<User>(_cancellationToken));
    Assert.Empty(await _repository.LoadAsync<User>([new StreamId(), StreamId.NewId()], _cancellationToken));
  }

  [Fact(DisplayName = "LoadAsync: it should return empty when no stream identifier was provided.")]
  public async Task Given_NoStreamId_When_LoadAsync_Then_Empty()
  {
    Assert.Empty(await _repository.LoadAsync<User>(ids: [], _cancellationToken));
  }

  [Fact(DisplayName = "LoadAsync: it should return null when the aggregate does not exist.")]
  public async Task Given_DoesNotExist_When_LoadAsync_Then_NullReturned()
  {
    Assert.Null(await _repository.LoadAsync<User>(StreamId.NewId(), _cancellationToken));
  }

  [Fact(DisplayName = "LoadAsync: it should return the aggregate up to the specified version.")]
  public async Task Given_VersionSpecified_When_LoadAsync_Then_AggregateUpToVersion()
  {
    long version = 1;

    StreamId userId = StreamId.NewId();
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = userId,
      Version = 1
    };
    Stream stream = new(userId, typeof(User), [new Event(created.Id, created.Version, created.OccurredOn, created, created.ActorId, created.IsDeleted)]);
    _eventStore.Setup(x => x.FetchAsync(stream.Id, It.Is<FetchOptions>(y => y.ToVersion == version), _cancellationToken)).ReturnsAsync(stream);

    User? user = await _repository.LoadAsync<User>(stream.Id, version, _cancellationToken);
    Assert.NotNull(user);
    Assert.Equal(stream.Id, user.Id);
    Assert.Equal(version, user.Version);
  }

  [Fact(DisplayName = "LoadAsync: it should return the aggregate found.")]
  public async Task Given_AggregateFound_When_LoadAsync_Then_AggregateReturned()
  {
    StreamId userId = StreamId.NewId();
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = userId,
      Version = 1
    };
    Stream stream = new(userId, typeof(User), [new Event(created.Id, created.Version, created.OccurredOn, created, created.ActorId, created.IsDeleted)]);
    _eventStore.Setup(x => x.FetchAsync(stream.Id, It.IsAny<FetchOptions>(), _cancellationToken)).ReturnsAsync(stream);

    User? user = await _repository.LoadAsync<User>(stream.Id, _cancellationToken);
    Assert.NotNull(user);
    Assert.Equal(stream.Id, user.Id);
  }

  [Theory(DisplayName = "LoadAsync: it should return the aggregate when the deletion flag was matched.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_DeletionFlagMatching_When_LoadAsync_Then_AggregateReturned(bool isDeleted)
  {
    StreamId userId = StreamId.NewId();
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = userId,
      Version = 1
    };
    Stream stream = new(userId, typeof(User), [new Event(created.Id, created.Version, created.OccurredOn, created, created.ActorId, isDeleted)]);
    _eventStore.Setup(x => x.FetchAsync(stream.Id, It.Is<FetchOptions>(y => y.IsDeleted == isDeleted), _cancellationToken)).ReturnsAsync(stream);

    User? user = await _repository.LoadAsync<User>(stream.Id, isDeleted, _cancellationToken);
    Assert.NotNull(user);
    Assert.Equal(stream.Id, user.Id);
  }

  [Fact(DisplayName = "SaveAsync: it should append, clear then save the changes of an aggregate.")]
  public async Task Given_AnAggregate_When_SaveAsync_Then_ChangesAreAppendedClearedAndSaved()
  {
    User user = new(_faker.Person.UserName);
    user.SignIn();
    IEvent[] changes = [.. user.Changes];

    await _repository.SaveAsync(user, _cancellationToken);

    _eventStore.Verify(x => x.Append(user.Id, typeof(User), StreamExpectation.ShouldBeAtVersion(user.Version), It.Is<IEnumerable<IEvent>>(y => y.SequenceEqual(changes))), Times.Once);

    Assert.False(user.HasChanges);
    Assert.Empty(user.Changes);

    _eventStore.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should append, clear then save the changes of many aggregates.")]
  public async Task Given_ManyAggregates_When_SaveAsync_Then_ChangesAreAppendedClearedAndSaved()
  {
    User user = new(_faker.Person.UserName);
    user.SignIn();
    IEvent[] userChanges = [.. user.Changes];

    Token token = Token.Generate();
    IEvent[] token1Changes = [.. token.Changes];

    Token tokenWithoutChanges = Token.Generate();
    tokenWithoutChanges.ClearChanges();
    IEvent[] token2Changes = [.. tokenWithoutChanges.Changes];

    await _repository.SaveAsync([user, token], _cancellationToken);

    _eventStore.Verify(x => x.Append(user.Id, typeof(User), StreamExpectation.ShouldBeAtVersion(user.Version), It.Is<IEnumerable<IEvent>>(y => y.SequenceEqual(userChanges))), Times.Once);
    _eventStore.Verify(x => x.Append(token.Id, typeof(Token), StreamExpectation.None, It.Is<IEnumerable<IEvent>>(y => y.SequenceEqual(token1Changes))), Times.Once);
    _eventStore.Verify(x => x.Append(tokenWithoutChanges.Id, typeof(Token), StreamExpectation.None, It.Is<IEnumerable<IEvent>>(y => y.SequenceEqual(token2Changes))), Times.Never);

    Assert.False(user.HasChanges);
    Assert.Empty(user.Changes);

    Assert.False(token.HasChanges);
    Assert.Empty(token.Changes);

    _eventStore.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should not do anything where there is no change.")]
  public async Task Given_NoChange_When_SaveAsync_Then_DoNothing()
  {
    User user = new(_faker.Person.UserName);
    user.SignIn();
    user.ClearChanges();

    await _repository.SaveAsync(user, _cancellationToken);
    _eventStore.Verify(x => x.Append(It.IsAny<StreamId>(), It.IsAny<Type>(), It.IsAny<StreamExpectation>(), It.IsAny<IEnumerable<IEvent>>()), Times.Never);
    _eventStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

    await _repository.SaveAsync([], _cancellationToken);
    _eventStore.Verify(x => x.Append(It.IsAny<StreamId>(), It.IsAny<Type>(), It.IsAny<StreamExpectation>(), It.IsAny<IEnumerable<IEvent>>()), Times.Never);
    _eventStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

    await _repository.SaveAsync([user], _cancellationToken);
    _eventStore.Verify(x => x.Append(It.IsAny<StreamId>(), It.IsAny<Type>(), It.IsAny<StreamExpectation>(), It.IsAny<IEnumerable<IEvent>>()), Times.Never);
    _eventStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
  }

  private static Event ToEvent(IEvent @event)
  {
    EventId id = @event is IIdentifiableEvent identifiable ? identifiable.Id : EventId.NewId();
    long version = @event is IVersionedEvent versioned ? versioned.Version : 0;
    DateTime occurredOn = @event is ITemporalEvent temporal ? temporal.OccurredOn : DateTime.Now;
    ActorId? actorId = @event is IActorEvent actor ? actor.ActorId : null;

    bool? isDeleted = null;
    if (@event is IDeleteControlEvent control && control.IsDeleted.HasValue)
    {
      isDeleted = control.IsDeleted.Value;
    }
    else if (@event is IDeleteEvent && @event is not IUndeleteEvent)
    {
      isDeleted = true;
    }
    else if (@event is IUndeleteEvent && @event is not IDeleteEvent)
    {
      isDeleted = false;
    }

    return new Event(id, version, occurredOn, @event, actorId, isDeleted);
  }
}
