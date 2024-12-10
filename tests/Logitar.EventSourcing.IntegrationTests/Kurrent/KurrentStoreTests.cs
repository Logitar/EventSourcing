using Bogus;
using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Kurrent;

[Trait(Traits.Category, Categories.Integration)]
public class KurrentStoreTests : KurrentIntegrationTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly IEventStore _store;

  public KurrentStoreTests() : base()
  {
    _serializerOptions.Converters.Add(new ActorIdConverter());
    _serializerOptions.Converters.Add(new EventIdConverter());
    _serializerOptions.Converters.Add(new StreamIdConverter());
    _serializerOptions.Converters.Add(new TypeConverter());

    _store = ServiceProvider.GetRequiredService<IEventStore>();
  }

  [Fact(DisplayName = "SaveChangesAsync: it should convert the appended events correctly.")]
  public async Task Given_AppendedEvents_When_SaveChangesAsync_Then_EventsAreConvertedCorrectly()
  {
    StreamId userId = StreamId.NewId();
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = userId,
      Version = 1,
      ActorId = new ActorId(_faker.Internet.UserName()),
      IsDeleted = false
    };
    _store.Append(userId, typeof(User), StreamExpectation.ShouldNotExist, [created]);

    await _store.SaveChangesAsync(_cancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: _cancellationToken);
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
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);

    await _store.SaveChangesAsync(_cancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: _cancellationToken);
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
    UserCreated created = new(_faker.Person.UserName);
    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldBeAtVersion(2), [created, passwordCreated]);

    await _store.SaveChangesAsync(_cancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: _cancellationToken);
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
    UserCreated created = new(_faker.Person.UserName);
    UserSignedIn signedIn = new();
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.None, [created, signedIn]);

    await _store.SaveChangesAsync(_cancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: _cancellationToken);
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
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);

    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    UserSignedIn signedIn = new();
    _store.Append(userId, typeof(User), StreamExpectation.ShouldBeAtVersion(4), [passwordCreated, signedIn]);

    var exception = await Assert.ThrowsAsync<WrongExpectedVersionException>(async () => await _store.SaveChangesAsync(_cancellationToken));
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

    var exception = await Assert.ThrowsAsync<WrongExpectedVersionException>(async () => await _store.SaveChangesAsync(_cancellationToken));
    Assert.Equal(userId.Value, exception.StreamName);
    Assert.Null(exception.ExpectedVersion);
    Assert.Null(exception.ActualVersion);
    Assert.Equal(StreamRevision.None, exception.ExpectedStreamRevision);
    Assert.Equal(StreamRevision.None, exception.ActualStreamRevision);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should throw WrongExpectedVersionException when the stream should not exist but it exists.")]
  public async Task Given_ShouldNotExistStreamExists_When_SaveChangesAsync_Then_WrongExpectedVersionExceptionIsThrown()
  {
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(_cancellationToken);

    _store.Append(userId, typeof(User), StreamExpectation.ShouldNotExist, [new UserCreated(_faker.Internet.UserName())]);
    var exception = await Assert.ThrowsAsync<WrongExpectedVersionException>(async () => await _store.SaveChangesAsync(_cancellationToken));
    Assert.Equal(userId.Value, exception.StreamName);
    Assert.Null(exception.ExpectedVersion);
    Assert.Equal(0, exception.ActualVersion);
    Assert.Equal(StreamRevision.None, exception.ExpectedStreamRevision);
    Assert.Equal(StreamRevision.FromInt64(0), exception.ActualStreamRevision);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should update an existing stream when the expected version is verified.")]
  public async Task Given_ExpectedVersionVerified_When_SaveChangesAsync_Then_EventsAppendedToExistingStream()
  {
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(_cancellationToken);

    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    UserSignedIn signedIn = new();
    _store.Append(userId, typeof(User), StreamExpectation.ShouldBeAtVersion(3), [passwordCreated, signedIn]);

    await _store.SaveChangesAsync(_cancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: _cancellationToken);
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
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(_cancellationToken);

    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    _store.Append(userId, typeof(User), StreamExpectation.ShouldExist, [passwordCreated]);

    await _store.SaveChangesAsync(_cancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: _cancellationToken);
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
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(_cancellationToken);

    UserSignedIn signedIn = new();
    _store.Append(userId, typeof(User), StreamExpectation.None, [signedIn]);

    await _store.SaveChangesAsync(_cancellationToken);

    EventStoreClient.ReadStreamResult result = EventStoreClient.ReadStreamAsync(Direction.Forwards, userId.Value, StreamPosition.Start, cancellationToken: _cancellationToken);
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
