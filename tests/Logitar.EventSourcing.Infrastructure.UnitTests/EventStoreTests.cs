using Bogus;
using Moq;

namespace Logitar.EventSourcing.Infrastructure;

[Trait(Traits.Category, Categories.Unit)]
public class EventStoreTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IEventBus> _bus1 = new();
  private readonly Mock<IEventBus> _bus2 = new();

  private readonly FakeEventStore _store;

  public EventStoreTests()
  {
    _store = new FakeEventStore([_bus1.Object, _bus2.Object]);
  }

  [Fact(DisplayName = "Append: it should generate a stream identifier when none is provided.")]
  public void Given_NoStreamId_When_Append_Then_StreamIdGenerated()
  {
    StreamId streamId = _store.Append(streamId: null, streamType: null, StreamExpectation.None, events: []);
    Assert.NotEmpty(streamId.Value);
  }

  [Fact(DisplayName = "Append: it should not track any change when there is no event.")]
  public void Given_NoEvent_When_Append_Then_NoChangeTracked()
  {
    Assert.False(_store.HasChanges);
    _store.Append(StreamId.NewId(), streamType: null, StreamExpectation.None, events: []);
    Assert.False(_store.HasChanges);
  }

  [Fact(DisplayName = "Append: it should throw ArgumentException when the stream identifier value is empty.")]
  public void Given_EmptyStreamId_When_Append_Then_ArgumentException()
  {
    StreamId streamId = new();
    var exception = Assert.Throws<ArgumentException>(() => _store.Append(streamId, streamType: null, StreamExpectation.None, events: []));
    Assert.StartsWith("The stream identifier is required.", exception.Message);
    Assert.Equal("streamId", exception.ParamName);
  }

  [Fact(DisplayName = "Append: it should track a new AppendToStream operation.")]
  public void Given_Events_When_Append_Then_AppendToStream()
  {
    User user = new(_faker.Person.UserName);
    user.SignIn();

    StreamId userId = _store.Append(user.Id, user.GetType(), StreamExpectation.ShouldBeAtVersion(user.Version), user.Changes);
    Assert.Equal(user.Id, userId);

    Assert.True(_store.HasChanges);

    AppendToStream stream = _store.PeekChange();
    Assert.Equal(user.Id, stream.Id);
    Assert.Equal(user.GetType(), stream.Type);
    Assert.Equal(StreamExpectation.StreamExpectationKind.ShouldBeAtVersion, stream.Expectation.Kind);
    Assert.Equal(user.Version, stream.Expectation.Version);
    Assert.Equal(user.Changes, stream.Events);
  }

  [Fact(DisplayName = "ClearChanges: it should clear any tracked changes.")]
  public void Given_TrackedChanges_When_ClearChanges_Then_NoTrackedChange()
  {
    User user = new(_faker.Person.UserName);
    _store.Append(user.Id, user.GetType(), StreamExpectation.ShouldBeAtVersion(user.Version), user.Changes);
    Assert.True(_store.HasChanges);

    _store.ClearChanges();
    Assert.False(_store.HasChanges);
  }

  [Fact(DisplayName = "ClearChanges: it should not do anything when there is no change.")]
  public void Given_NoChange_When_ClearChanges_Then_DoNothing()
  {
    Assert.False(_store.HasChanges);
    _store.ClearChanges();
    Assert.False(_store.HasChanges);
  }

  [Fact(DisplayName = "DequeueChange: it should dequeue and return the next tracked change.")]
  public void Given_TrackedChange_When_DequeueChange_Then_ChangeDequeued()
  {
    User user = new(_faker.Person.UserName);
    _store.Append(user.Id, user.GetType(), StreamExpectation.ShouldBeAtVersion(user.Version), user.Changes);
    Assert.True(_store.HasChanges);

    AppendToStream stream = _store.DequeueChange();
    Assert.False(_store.HasChanges);

    Assert.Equal(user.Id, stream.Id);
    Assert.Equal(user.GetType(), stream.Type);
    Assert.Equal(StreamExpectation.StreamExpectationKind.ShouldBeAtVersion, stream.Expectation.Kind);
    Assert.Equal(user.Version, stream.Expectation.Version);
    Assert.Equal(user.Changes, stream.Events);
  }

  [Fact(DisplayName = "PublishAsync: it should not publish any event when there is no event bus.")]
  public async Task Given_NoEventBus_When_PublishAsync_Then_NoEventPublished()
  {
    Queue<IEvent> events = [];
    events.Enqueue(new UserCreated(_faker.Person.UserName));

    FakeEventStore store = new(buses: []);
    await store.PublishAsync(events, _cancellationToken);
    Assert.Empty(events);
  }

  [Fact(DisplayName = "PublishAsync: it should not publish any event when there is no event.")]
  public async Task Given_NoEvent_When_PublishAsync_Then_NoEventPublished()
  {
    Queue<IEvent> events = [];

    await _store.PublishAsync(events, _cancellationToken);
    Assert.Empty(events);

    _bus1.Verify(x => x.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    _bus2.Verify(x => x.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "PublishAsync: ")]
  public async Task Given_EventsAndBuses_When_PublishAsync_Then_EventsPublished()
  {
    Queue<IEvent> events = [];

    IEvent[] changes = [new UserCreated(_faker.Person.UserName), new UserSignedIn()];
    foreach (IEvent change in changes)
    {
      events.Enqueue(change);
    }

    await _store.PublishAsync(events, _cancellationToken);
    Assert.Empty(events);

    foreach (IEvent change in changes)
    {
      _bus1.Verify(x => x.PublishAsync(change, _cancellationToken), Times.Once);
      _bus2.Verify(x => x.PublishAsync(change, _cancellationToken), Times.Once);
    }
  }
}
