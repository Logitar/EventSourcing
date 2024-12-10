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

  [Fact(DisplayName = "LoadAsync: it should only return the matching aggregates.")]
  public async Task Given_IdsAndDeletionFlag_When_LoadAsync_Then_OnlyReturnsMatchingAggregates()
  {
    User user = new(_faker.Person.UserName);

    User deleted = new(_faker.Internet.UserName());
    deleted.Delete();

    Stream userStream = new(user.Id, typeof(User), user.Changes.Select(ToEvent));
    _eventStore.Setup(x => x.FetchAsync(user.Id, It.IsAny<FetchOptions>(), _cancellationToken)).ReturnsAsync(userStream);

    Stream deletedStream = new(deleted.Id, typeof(User), deleted.Changes.Select(ToEvent));
    _eventStore.Setup(x => x.FetchAsync(deleted.Id, It.IsAny<FetchOptions>(), _cancellationToken)).ReturnsAsync(deletedStream);

    IReadOnlyCollection<User> users = await _repository.LoadAsync<User>([user.Id, deleted.Id, StreamId.NewId()], isDeleted: false, _cancellationToken);
    Assert.Equal(user, Assert.Single(users));
  }

  [Fact(DisplayName = "LoadAsync: it should return empty when no aggregate was found.")]
  public async Task Given_NoAggregateFound_When_LoadAsync_Then_Empty()
  {
    IReadOnlyCollection<User> users = await _repository.LoadAsync<User>([StreamId.NewId(), StreamId.NewId()], _cancellationToken);
    Assert.Empty(users);
  }

  [Fact(DisplayName = "LoadAsync: it should return null when the aggregate does not exist.")]
  public async Task Given_DoesNotExisting_When_LoadAsync_Then_NullReturned()
  {
    Assert.Null(await _repository.LoadAsync<User>(StreamId.NewId(), _cancellationToken));
  }

  [Theory(DisplayName = "LoadAsync: it should return null when the deletion flag was not matched.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_DeletionFlagNotMatching_When_LoadAsync_Then_NullReturned(bool isDeleted)
  {
    Event[] events =
    [
      new Event(EventId.NewId(), version: 1, occurredOn: DateTime.Now, new UserCreated(_faker.Person.UserName)),
      new Event(EventId.NewId(), version: 2, occurredOn: DateTime.Now, new UserSignedIn(), actorId: null, !isDeleted)
    ];
    Stream stream = new(StreamId.NewId(), typeof(User), events);
    _eventStore.Setup(x => x.FetchAsync(stream.Id, It.IsAny<FetchOptions>(), _cancellationToken)).ReturnsAsync(stream);

    Assert.Null(await _repository.LoadAsync<User>(stream.Id, isDeleted, _cancellationToken));
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

  [Fact(DisplayName = "LoadAsync: it should return the aggregate it was found.")]
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
    _eventStore.Setup(x => x.FetchAsync(stream.Id, It.IsAny<FetchOptions>(), _cancellationToken)).ReturnsAsync(stream);

    User? user = await _repository.LoadAsync<User>(stream.Id, _cancellationToken);
    Assert.NotNull(user);
    Assert.Equal(stream.Id, user.Id);
  }

  [Theory(DisplayName = "LoadAsync: it should throw NotSupportedException when loading all aggregates of the same type.")]
  [InlineData(null)]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_AllAggregatesSameType_When_LoadAsync_Then_NotSupportedException(bool? isDeleted)
  {
    var exception = await Assert.ThrowsAsync<NotSupportedException>(async () => await _repository.LoadAsync<User>(isDeleted, _cancellationToken));
    Assert.Equal("Loading all aggregates of the specified type is not supported by the current repository.", exception.Message);

    if (isDeleted == null)
    {
      exception = await Assert.ThrowsAsync<NotSupportedException>(async () => await _repository.LoadAsync<User>(_cancellationToken));
      Assert.Equal("Loading all aggregates of the specified type is not supported by the current repository.", exception.Message);
    }
  }

  [Fact(DisplayName = "SaveAsync: it should append, clear then save the changes of an aggregate.")]
  public async Task Given_AnAggregate_When_SaveAsync_Then_ChangesAreAppendedClearedAndSaved()
  {
    User user = new(_faker.Person.UserName);
    user.SignIn();

    await _repository.SaveAsync(user, _cancellationToken);

    _eventStore.Verify(x => x.Append(user.Id, typeof(User), StreamExpectation.ShouldBeAtVersion(user.Version), user.Changes), Times.Once);

    Assert.False(user.HasChanges);
    Assert.Empty(user.Changes);

    _eventStore.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should append, clear then save the changes of many aggregates.")]
  public async Task Given_ManyAggregates_When_SaveAsync_Then_ChangesAreAppendedClearedAndSaved()
  {
    User user = new(_faker.Person.UserName);
    user.SignIn();

    Token token = Token.Generate();
    Token tokenWithoutChanges = Token.Generate();
    tokenWithoutChanges.ClearChanges();

    await _repository.SaveAsync([user, token], _cancellationToken);

    _eventStore.Verify(x => x.Append(user.Id, typeof(User), StreamExpectation.ShouldBeAtVersion(user.Version), user.Changes), Times.Once);
    _eventStore.Verify(x => x.Append(token.Id, typeof(Token), StreamExpectation.None, token.Changes), Times.Once);
    _eventStore.Verify(x => x.Append(tokenWithoutChanges.Id, typeof(Token), StreamExpectation.None, tokenWithoutChanges.Changes), Times.Never);

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
    DomainEvent domainEvent = (DomainEvent)@event;

    bool? isDeleted = domainEvent.IsDeleted;
    if (isDeleted == null)
    {
      if (@event is IDeleteEvent && @event is not IUndeleteEvent)
      {
        isDeleted = true;
      }
      else if (@event is IUndeleteEvent && @event is not IDeleteEvent)
      {
        isDeleted = false;
      }
    }

    return new Event(domainEvent.Id, domainEvent.Version, domainEvent.OccurredOn, domainEvent, domainEvent.ActorId, isDeleted);
  }
}
