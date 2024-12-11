using Bogus;
using Marten;
using Marten.Events;
using Marten.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.MartenDB;

[Trait(Traits.Category, Categories.Integration)]
public class MartenStoreTests : MartenIntegrationTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly IEventStore _store;

  public MartenStoreTests() : base()
  {
    _store = ServiceProvider.GetRequiredService<IEventStore>();
  }

  [Fact(Skip = "TODO(fpion): implement", DisplayName = "SaveChangesAsync: it should create a new stream when it does not exist and should not exist.")]
  public async Task Given_ShouldNotExistNoStream_When_SaveChangesAsync_Then_NewStreamCreated()
  {
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);

    await _store.SaveChangesAsync(_cancellationToken);

    using IDocumentSession session = await DocumentStore.LightweightSerializableSessionAsync(_cancellationToken);

    IReadOnlyList<Marten.Events.IEvent> events = await session.Events.FetchStreamAsync(userId.ToGuid(), token: _cancellationToken);
    Assert.Single(events);

    //StreamState? stream = await session.Events.FetchStreamStateAsync(userId.ToGuid(), _cancellationToken);
    //Assert.NotNull(stream);
    //Assert.Equal(userId.ToGuid(), stream.Id);
    //Assert.Equal(1, stream.Version);
    //Assert.Equal(typeof(User), stream.AggregateType);
    //Assert.Equal(DateTimeOffset.UtcNow, stream.LastTimestamp, TimeSpan.FromSeconds(1));
    //Assert.Equal(DateTimeOffset.UtcNow, stream.Created, TimeSpan.FromSeconds(1));
    //Assert.Null(stream.Key);
    //Assert.False(stream.IsArchived); // TODO(fpion): implement

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created]);
  }

  [Fact(Skip = "TODO(fpion): implement", DisplayName = "SaveChangesAsync: it should create a new stream when the version equals the event count.")]
  public async Task Given_ExpectedVersionEqualEvents_When_SaveChangesAsync_Then_NewStreamCreated()
  {
    UserCreated created = new(_faker.Person.UserName);
    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldBeAtVersion(2), [created, passwordCreated]);

    await _store.SaveChangesAsync(_cancellationToken);

    using IDocumentSession session = await DocumentStore.LightweightSerializableSessionAsync(_cancellationToken);

    IReadOnlyList<Marten.Events.IEvent> events = await session.Events.FetchStreamAsync(userId.ToGuid(), token: _cancellationToken);
    Assert.Equal(2, events.Count);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created, passwordCreated]);
  }

  [Fact(Skip = "TODO(fpion): implement", DisplayName = "SaveChangesAsync: it should create a new stream when there is no stream expectation.")]
  public async Task Given_NoExpectationNoStream_When_SaveChangesAsync_Then_NewStreamCreated()
  {
    UserCreated created = new(_faker.Person.UserName);
    UserSignedIn signedIn = new();
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.None, [created, signedIn]);

    await _store.SaveChangesAsync(_cancellationToken);

    using IDocumentSession session = await DocumentStore.LightweightSerializableSessionAsync(_cancellationToken);

    IReadOnlyList<Marten.Events.IEvent> events = await session.Events.FetchStreamAsync(userId.ToGuid(), token: _cancellationToken);
    Assert.Equal(2, events.Count);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created, signedIn]);
  }

  [Fact(Skip = "TODO(fpion): implement", DisplayName = "SaveChangesAsync: it should create a new stream with a string ID (StreamKey) and no Guid ID (StreamId).")]
  public async Task Given_IdIsNotGuid_When_SaveChangesAsync_Then_NewStreamCreatedWithKey()
  {
    StreamId userId = new(_faker.Person.UserName.ToUpperInvariant());
    UserCreated created = new(_faker.Person.UserName);
    _store.Append(userId, typeof(User), StreamExpectation.ShouldNotExist, [created]);

    await _store.SaveChangesAsync(_cancellationToken);

    using IDocumentSession session = await DocumentStore.LightweightSerializableSessionAsync(_cancellationToken);

    IReadOnlyList<Marten.Events.IEvent> events = await session.Events.FetchStreamAsync(userId.ToGuid(), token: _cancellationToken);
    Assert.Single(events);

    StreamState? stream = await session.Events.FetchStreamStateAsync(userId.ToGuid(), _cancellationToken);
    Assert.NotNull(stream);
    Assert.Equal(Guid.Empty, stream.Id);
    Assert.Equal(userId.Value, stream.Key);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created]);
  }

  [Fact(Skip = "TODO(fpion): implement", DisplayName = "SaveChangesAsync: it should throw EventStreamUnexpectedMaxEventIdException when the actual stream version differs from the expected.")]
  public async Task Given_ShouldBeAtVersionDiffers_When_SaveChangesAsync_Then_EventStreamUnexpectedMaxEventIdException()
  {
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);

    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    UserSignedIn signedIn = new();
    _store.Append(userId, typeof(User), StreamExpectation.ShouldBeAtVersion(4), [passwordCreated, signedIn]);

    var exception = await Assert.ThrowsAsync<EventStreamUnexpectedMaxEventIdException>(async () => await _store.SaveChangesAsync(_cancellationToken));
    Assert.Equal(typeof(User), exception.AggregateType);
    Assert.Equal("Logitar.EventSourcing.User", exception.DocType);
    Assert.Equal(userId.ToGuid(), exception.Id);
  }

  [Fact(Skip = "TODO(fpion): implement", DisplayName = "SaveChangesAsync: it should throw EventStreamUnexpectedMaxEventIdException when the stream should exist but it does not.")]
  public async Task Given_ShouldExistNoStream_When_SaveChangesAsync_Then_EventStreamUnexpectedMaxEventIdException()
  {
    StreamId userId = StreamId.NewId();
    _store.Append(userId, typeof(User), StreamExpectation.ShouldExist, [new UserSignedIn()]);

    var exception = await Assert.ThrowsAsync<EventStreamUnexpectedMaxEventIdException>(async () => await _store.SaveChangesAsync(_cancellationToken));
    Assert.Equal(typeof(User), exception.AggregateType);
    Assert.Equal("Logitar.EventSourcing.User", exception.DocType);
    Assert.Equal(userId.ToGuid(), exception.Id);
  }

  [Fact(Skip = "TODO(fpion): implement", DisplayName = "SaveChangesAsync: it should throw ExistingStreamIdCollisionException when the stream should not exist but it exists.")]
  public async Task Given_ShouldNotExistStreamExists_When_SaveChangesAsync_Then_ExistingStreamIdCollisionException()
  {
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(_cancellationToken);

    _store.Append(userId, typeof(User), StreamExpectation.ShouldNotExist, [new UserCreated(_faker.Internet.UserName())]);
    var exception = await Assert.ThrowsAsync<ExistingStreamIdCollisionException>(async () => await _store.SaveChangesAsync(_cancellationToken));
    Assert.Equal(typeof(User), exception.AggregateType);
    Assert.Equal(userId.ToGuid(), exception.Id);
  }

  [Fact(Skip = "TODO(fpion): implement", DisplayName = "SaveChangesAsync: it should update an existing stream when the expected version is verified.")]
  public async Task Given_ExpectedVersionVerified_When_SaveChangesAsync_Then_EventsAppendedToExistingStream()
  {
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(_cancellationToken);

    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    UserSignedIn signedIn = new();
    _store.Append(userId, typeof(User), StreamExpectation.ShouldBeAtVersion(3), [passwordCreated, signedIn]);

    await _store.SaveChangesAsync(_cancellationToken);

    using IDocumentSession session = await DocumentStore.LightweightSerializableSessionAsync(_cancellationToken);

    IReadOnlyList<Marten.Events.IEvent> events = await session.Events.FetchStreamAsync(userId.ToGuid(), token: _cancellationToken);
    Assert.Equal(3, events.Count);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created, passwordCreated, signedIn]);
  }

  [Fact(Skip = "TODO(fpion): implement", DisplayName = "SaveChangesAsync: it should update an existing stream when the stream exists and should exist.")]
  public async Task Given_ShouldExistStreamExists_When_SaveChangesAsync_Then_EventsAppendedToExistingStream()
  {
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(_cancellationToken);

    UserPasswordCreated passwordCreated = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")));
    _store.Append(userId, typeof(User), StreamExpectation.ShouldExist, [passwordCreated]);

    await _store.SaveChangesAsync(_cancellationToken);

    using IDocumentSession session = await DocumentStore.LightweightSerializableSessionAsync(_cancellationToken);

    IReadOnlyList<Marten.Events.IEvent> events = await session.Events.FetchStreamAsync(userId.ToGuid(), token: _cancellationToken);
    Assert.Equal(2, events.Count);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created, passwordCreated]);
  }

  [Fact(Skip = "TODO(fpion): implement", DisplayName = "SaveChangesAsync: it should update an existing stream when there is no stream expectation.")]
  public async Task Given_NoExpectationStreamExists_When_SaveChangesAsync_Then_EventsAppendedToExistingStream()
  {
    UserCreated created = new(_faker.Person.UserName);
    StreamId userId = _store.Append(streamId: null, typeof(User), StreamExpectation.ShouldNotExist, [created]);
    await _store.SaveChangesAsync(_cancellationToken);

    UserSignedIn signedIn = new();
    _store.Append(userId, typeof(User), StreamExpectation.None, [signedIn]);

    await _store.SaveChangesAsync(_cancellationToken);

    using IDocumentSession session = await DocumentStore.LightweightSerializableSessionAsync(_cancellationToken);

    IReadOnlyList<Marten.Events.IEvent> events = await session.Events.FetchStreamAsync(userId.ToGuid(), token: _cancellationToken);
    Assert.Equal(2, events.Count);

    Assert.False(_store.HasChanges);
    EventBus.VerifyPublished([created, signedIn]);
  }
}
