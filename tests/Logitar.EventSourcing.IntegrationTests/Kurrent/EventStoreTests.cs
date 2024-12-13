using Bogus;
using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Kurrent;

[Trait(Traits.Category, Categories.Integration)]
public class EventStoreTests : KurrentIntegrationTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly IEventStore _store;

  public EventStoreTests() : base()
  {
    _serializerOptions.Converters.Add(new ActorIdConverter());
    _serializerOptions.Converters.Add(new EventIdConverter());
    _serializerOptions.Converters.Add(new StreamIdConverter());
    _serializerOptions.Converters.Add(new TypeConverter());

    _store = ServiceProvider.GetRequiredService<IEventStore>();
  }

  [Fact(DisplayName = "FetchAsync: it should return empty when no event was found (many).")]
  public async Task Given_NoStream_When_FetchManyAsync_Then_EmptyReturned()
  {
    DateTime now = DateTime.Now;

    User user = new(Faker.Person.UserName);
    _store.Append(user.Id, user.GetType(), StreamExpectation.ShouldBeAtVersion(user.Version), user.Changes);
    await _store.SaveChangesAsync(CancellationToken);

    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(new FetchManyOptions
    {
      StreamTypes = [typeof(Session)],
      OccurredFrom = now // NOTE(fpion): this is required because of data from another tests.
    }, CancellationToken);
    Assert.Empty(streams);
  }

  [Fact(DisplayName = "FetchAsync: it should return null when no event was found (single).")]
  public async Task Given_NoStream_When_FetchSingleAsync_Then_NullReturned()
  {
    User user = new(Faker.Person.UserName);
    _store.Append(user.Id, user.GetType(), StreamExpectation.ShouldBeAtVersion(user.Version), user.Changes);
    await _store.SaveChangesAsync(CancellationToken);

    Stream? stream = await _store.FetchAsync(user.Id, new FetchOptions
    {
      FromVersion = user.Version + 1
    }, CancellationToken);
    Assert.Null(stream);
  }

  [Fact(DisplayName = "FetchAsync: it should return null when the stream identifier is empty.")]
  public async Task Given_StreamIdEmpty_When_FetchSingleAsync_Then_NullReturned()
  {
    Stream? stream = await _store.FetchAsync(new StreamId(), options: null, CancellationToken);
    Assert.Null(stream);
  }

  [Fact(DisplayName = "FetchAsync: it should return the correct stream when events were found (single).")]
  public async Task Given_StreamsAndEvents_When_FetchSingleAsync_Then_CorrectStreamReturned()
  {
    User user = new(Faker.Person.UserName);

    user.SignIn(new ActorId(user.Id.Value));
    UserSignedIn? signedIn = user.Changes.Last() as UserSignedIn;
    Assert.NotNull(signedIn);

    user.Disable(new ActorId(Guid.Empty));
    UserDisabled? disabled = user.Changes.Last() as UserDisabled;
    Assert.NotNull(disabled);

    user.Delete(new ActorId("SYSTEM"));
    _store.Append(user.Id, user.GetType(), StreamExpectation.ShouldBeAtVersion(user.Version), user.Changes);
    await _store.SaveChangesAsync(CancellationToken);

    Stream? stream = await _store.FetchAsync(user.Id, new FetchOptions
    {
      FromVersion = 2,
      ToVersion = 3
    }, CancellationToken);
    Assert.NotNull(stream);

    Assert.Equal(user.Id, stream.Id);
    Assert.Equal(typeof(User), stream.Type);
    Assert.Equal(3, stream.Version);
    Assert.Null(stream.CreatedBy);
    Assert.Null(stream.CreatedOn);
    Assert.Equal(disabled.ActorId, stream.UpdatedBy);
    Assert.True(stream.UpdatedOn.HasValue);
    Assert.Equal(disabled.OccurredOn, stream.UpdatedOn.Value, TimeSpan.FromSeconds(1));
    Assert.False(stream.IsDeleted);

    Assert.Equal(2, stream.Events.Count);
    Assert.Contains(stream.Events, e => e.Id == signedIn.Id && e.Version == signedIn.Version && e.ActorId == signedIn.ActorId
      && (signedIn.OccurredOn - e.OccurredOn.AsUniversalTime() < TimeSpan.FromSeconds(1)) && e.IsDeleted == signedIn.IsDeleted && e.Data.Equals(signedIn));
    Assert.Contains(stream.Events, e => e.Id == disabled.Id && e.Version == disabled.Version && e.ActorId == disabled.ActorId
      && (disabled.OccurredOn - e.OccurredOn.AsUniversalTime() < TimeSpan.FromSeconds(1)) && e.IsDeleted == null && e.Data.Equals(disabled));
  }

  [Fact(DisplayName = "FetchAsync: it should return the correct streams when events were found (many).")]
  public async Task Given_StreamsAndEvents_When_FetchManyAsync_Then_CorrectStreamReturned()
  {
    UserCreated userCreated = new(Faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddDays(-1)
    };
    ActorId actorId = new(userCreated.StreamId.Value);
    userCreated.ActorId = actorId;
    UserSignedIn userSignedIn = new()
    {
      StreamId = userCreated.StreamId,
      Version = 2,
      OccurredOn = DateTime.Now.AddHours(-1),
      ActorId = actorId
    };
    _store.Append(userCreated.StreamId, typeof(User), StreamExpectation.ShouldBeAtVersion(2), [userCreated, userSignedIn]);

    SessionCreated sessionCreated = new(userCreated.StreamId)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddHours(-1),
      ActorId = actorId
    };
    _store.Append(sessionCreated.StreamId, typeof(Session), StreamExpectation.ShouldBeAtVersion(1), [sessionCreated]);

    UserDeleted userDeleted = new()
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddDays(-10),
      ActorId = actorId,
      IsDeleted = true
    };
    _store.Append(userDeleted.StreamId, typeof(User), StreamExpectation.ShouldBeAtVersion(1), [userDeleted]);

    RoleCreated roleCreated = new("admin")
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddDays(-20),
      ActorId = actorId
    };
    _store.Append(roleCreated.StreamId, typeof(Role), StreamExpectation.ShouldBeAtVersion(1), [roleCreated]);

    UserCreated oldUserCreated = new(Faker.Internet.UserName())
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddYears(-1),
      ActorId = actorId
    };
    _store.Append(oldUserCreated.StreamId, typeof(User), StreamExpectation.ShouldBeAtVersion(1), [oldUserCreated]);

    UserCreated newUserCreated = new(Faker.Internet.UserName())
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now,
      ActorId = actorId
    };
    _store.Append(newUserCreated.StreamId, typeof(User), StreamExpectation.ShouldBeAtVersion(1), [newUserCreated]);

    SessionCreated systemSession = new(userCreated.StreamId)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddDays(-3)
    };
    _store.Append(systemSession.StreamId, typeof(Session), StreamExpectation.ShouldBeAtVersion(1), [systemSession]);

    UserCreated unwantedUserCreated = new(Faker.Internet.UserName())
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddMinutes(-10),
      ActorId = actorId
    };
    _store.Append(unwantedUserCreated.StreamId, typeof(User), StreamExpectation.ShouldBeAtVersion(1), [unwantedUserCreated]);

    await _store.SaveChangesAsync(CancellationToken);

    FetchManyOptions options = new()
    {
      StreamTypes = [typeof(User), typeof(Session)],
      StreamIds =
      [
        userCreated.StreamId,
        sessionCreated.StreamId,
        userDeleted.StreamId,
        roleCreated.StreamId,
        oldUserCreated.StreamId,
        newUserCreated.StreamId,
        systemSession.StreamId,
        StreamId.NewId(),
        new StreamId()
      ],
      FromVersion = 1,
      ToVersion = 1,
      OccurredFrom = DateTime.Now.AddMonths(-1),
      OccurredTo = DateTime.Now.AddMinutes(-1),
      Actor = new ActorFilter(actorId),
      IsDeleted = false
    };
    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(options, CancellationToken);

    Assert.Equal(2, streams.Count);
    Assert.Contains(streams, s => s.Id == userCreated.StreamId && typeof(User).Equals(s.Type) && s.Version == 1 && s.CreatedBy == actorId && s.CreatedOn.HasValue
      && s.UpdatedBy == s.CreatedBy && s.UpdatedOn == s.CreatedOn && !s.IsDeleted && Assert.Single(s.Events).Id == userCreated.Id);
    Assert.Contains(streams, s => s.Id == sessionCreated.StreamId && typeof(Session).Equals(s.Type) && s.Version == 1 && s.CreatedBy == actorId
      && s.CreatedOn.HasValue && s.UpdatedBy == s.CreatedBy && s.UpdatedOn == s.CreatedOn && Assert.Single(s.Events).Id == sessionCreated.Id);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should convert the appended events correctly.")]
  public async Task Given_AppendedEvents_When_SaveChangesAsync_Then_EventsAreConvertedCorrectly()
  {
    StreamId userId = StreamId.NewId();
    UserCreated created = new(Faker.Person.UserName)
    {
      StreamId = userId,
      Version = 1,
      ActorId = new ActorId(Faker.Internet.UserName()),
      IsDeleted = false
    };
    _store.Append(userId, typeof(User), StreamExpectation.ShouldNotExist, [created]);

    await _store.SaveChangesAsync(CancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: CancellationToken);
    Assert.Equal(ReadState.Ok, await result.ReadState);

    int events = 0;
    await foreach (ResolvedEvent resolvedEvent in result)
    {
      if (events == 0)
      {
        EventRecord record = resolvedEvent.Event;

        Assert.Equal(userId.Value, record.EventStreamId);
        Assert.Equal(created.Id.ToGuid(), record.EventId.ToGuid());
        Assert.Equal(StreamPosition.FromInt64(0), record.EventNumber);
        Assert.Equal("UserCreated", record.EventType);
        Assert.Equal(DateTime.UtcNow, record.Created, TimeSpan.FromSeconds(1));
        Assert.Equal(MediaTypeNames.Application.Json, record.ContentType);

        string jsonData = Encoding.UTF8.GetString(record.Data.ToArray());
        IEvent deserialized = EventSerializer.Deserialize(typeof(UserCreated), jsonData);
        Assert.Equal(created, deserialized);

        string jsonMetadata = Encoding.UTF8.GetString(record.Metadata.ToArray());
        EventMetadata? metadata = JsonSerializer.Deserialize<EventMetadata>(jsonMetadata, _serializerOptions);
        Assert.NotNull(metadata);
        Assert.Equal(created.GetType(), metadata.EventType);
        Assert.Equal(created.Id, metadata.EventId);
        Assert.Equal(typeof(User), metadata.StreamType);
        Assert.Equal(created.Version, metadata.Version);
        Assert.Equal(created.ActorId, metadata.ActorId);
        Assert.Equal(created.OccurredOn, metadata.OccurredOn);
        Assert.Equal(created.IsDeleted, metadata.IsDeleted);
      }

      events++;
    }
    Assert.Equal(1, events);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created]);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should create a new stream when it does not exist and should not exist.")]
  public async Task Given_ShouldNotExistNoStream_When_SaveChangesAsync_Then_NewStreamCreated()
  {
    UserCreated created = new(Faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);

    await _store.SaveChangesAsync(CancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: CancellationToken);
    Assert.Equal(ReadState.Ok, await result.ReadState);

    int events = 0;
    await foreach (ResolvedEvent resolvedEvent in result)
    {
      events++;
    }
    Assert.Equal(1, events);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created]);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should create a new stream when the version equals the event count.")]
  public async Task Given_ExpectedVersionEqualEvents_When_SaveChangesAsync_Then_NewStreamCreated()
  {
    UserCreated created = new(Faker.Person.UserName);
    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldBeAtVersion(2), [created, passwordCreated]);

    await _store.SaveChangesAsync(CancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: CancellationToken);
    Assert.Equal(ReadState.Ok, await result.ReadState);

    int events = 0;
    await foreach (ResolvedEvent resolvedEvent in result)
    {
      events++;
    }
    Assert.Equal(2, events);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created, passwordCreated]);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should create a new stream when there is no stream expectation.")]
  public async Task Given_NoExpectationNoStream_When_SaveChangesAsync_Then_NewStreamCreated()
  {
    UserCreated created = new(Faker.Person.UserName);
    UserSignedIn signedIn = new();
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.None, [created, signedIn]);

    await _store.SaveChangesAsync(CancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: CancellationToken);
    Assert.Equal(ReadState.Ok, await result.ReadState);

    int events = 0;
    await foreach (ResolvedEvent resolvedEvent in result)
    {
      events++;
    }
    Assert.Equal(2, events);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created, signedIn]);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should throw WrongExpectedVersionException when the actual stream version differs from the expected.")]
  public async Task Given_ShouldBeAtVersionDiffers_When_SaveChangesAsync_Then_WrongExpectedVersionExceptionIsThrown()
  {
    UserCreated created = new(Faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);

    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    UserSignedIn signedIn = new();
    _store.Append(userId, typeof(User), StreamExpectation.ShouldBeAtVersion(4), [passwordCreated, signedIn]);

    var exception = await Assert.ThrowsAsync<WrongExpectedVersionException>(async () => await _store.SaveChangesAsync(CancellationToken));
    Assert.Equal(userId.Value, exception.StreamName);
    Assert.Equal(1, exception.ExpectedVersion);
    Assert.Equal(0, exception.ActualVersion);
    Assert.Equal(StreamRevision.FromInt64(1), exception.ExpectedStreamRevision);
    Assert.Equal(StreamRevision.FromInt64(0), exception.ActualStreamRevision);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should throw WrongExpectedVersionException when the stream should exist but it does not.")]
  public async Task Given_ShouldExistNoStream_When_SaveChangesAsync_Then_WrongExpectedVersionExceptionIsThrown()
  {
    StreamId userId = StreamId.NewId();
    _store.Append(userId, typeof(User), StreamExpectation.ShouldExist, [new UserSignedIn()]);

    var exception = await Assert.ThrowsAsync<WrongExpectedVersionException>(async () => await _store.SaveChangesAsync(CancellationToken));
    Assert.Equal(userId.Value, exception.StreamName);
    Assert.Null(exception.ExpectedVersion);
    Assert.Null(exception.ActualVersion);
    Assert.Equal(StreamRevision.None, exception.ExpectedStreamRevision);
    Assert.Equal(StreamRevision.None, exception.ActualStreamRevision);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should throw WrongExpectedVersionException when the stream should not exist but it exists.")]
  public async Task Given_ShouldNotExistStreamExists_When_SaveChangesAsync_Then_WrongExpectedVersionExceptionIsThrown()
  {
    UserCreated created = new(Faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(CancellationToken);

    _store.Append(userId, typeof(User), StreamExpectation.ShouldNotExist, [new UserCreated(Faker.Internet.UserName())]);
    var exception = await Assert.ThrowsAsync<WrongExpectedVersionException>(async () => await _store.SaveChangesAsync(CancellationToken));
    Assert.Equal(userId.Value, exception.StreamName);
    Assert.Null(exception.ExpectedVersion);
    Assert.Equal(0, exception.ActualVersion);
    Assert.Equal(StreamRevision.None, exception.ExpectedStreamRevision);
    Assert.Equal(StreamRevision.FromInt64(0), exception.ActualStreamRevision);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should update an existing stream when the expected version is verified.")]
  public async Task Given_ExpectedVersionVerified_When_SaveChangesAsync_Then_EventsAppendedToExistingStream()
  {
    UserCreated created = new(Faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(CancellationToken);

    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    UserSignedIn signedIn = new();
    _store.Append(userId, typeof(User), StreamExpectation.ShouldBeAtVersion(3), [passwordCreated, signedIn]);

    await _store.SaveChangesAsync(CancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: CancellationToken);
    Assert.Equal(ReadState.Ok, await result.ReadState);

    int events = 0;
    await foreach (ResolvedEvent resolvedEvent in result)
    {
      events++;
    }
    Assert.Equal(3, events);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created, passwordCreated, signedIn]);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should update an existing stream when the stream exists and should exist.")]
  public async Task Given_ShouldExistStreamExists_When_SaveChangesAsync_Then_EventsAppendedToExistingStream()
  {
    UserCreated created = new(Faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(CancellationToken);

    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    _store.Append(userId, typeof(User), StreamExpectation.ShouldExist, [passwordCreated]);

    await _store.SaveChangesAsync(CancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: CancellationToken);
    Assert.Equal(ReadState.Ok, await result.ReadState);

    int events = 0;
    await foreach (ResolvedEvent resolvedEvent in result)
    {
      events++;
    }
    Assert.Equal(2, events);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created, passwordCreated]);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should update an existing stream when there is no stream expectation.")]
  public async Task Given_NoExpectationStreamExists_When_SaveChangesAsync_Then_EventsAppendedToExistingStream()
  {
    UserCreated created = new(Faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(CancellationToken);

    UserSignedIn signedIn = new();
    _store.Append(userId, typeof(User), StreamExpectation.None, [signedIn]);

    await _store.SaveChangesAsync(CancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: CancellationToken);
    Assert.Equal(ReadState.Ok, await result.ReadState);

    int events = 0;
    await foreach (ResolvedEvent resolvedEvent in result)
    {
      events++;
    }
    Assert.Equal(2, events);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created, signedIn]);
  }
}
