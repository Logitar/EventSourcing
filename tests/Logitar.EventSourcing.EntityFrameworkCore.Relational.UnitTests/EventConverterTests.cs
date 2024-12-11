using Bogus;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

[Trait(Traits.Category, Categories.Unit)]
public class EventConverterTests
{
  private readonly Faker _faker = new();

  private readonly EventSerializer _serializer;

  private readonly EventConverter _converter;

  public EventConverterTests()
  {
    _serializer = new EventSerializer();
    _converter = new EventConverter(_serializer);
  }

  [Theory(DisplayName = "ToEvent: it should convert the event entity to the correct event.")]
  [InlineData(null, null)]
  [InlineData("SYSTEM", true)]
  public void Given_EventEntity_When_ToEvent_Then_CorrectEvent(string? actorIdValue, bool? isDeleted)
  {
    StreamEntity stream = new(StreamId.NewId(), typeof(User));

    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = new StreamId(stream.Id),
      Version = 1
    };
    string typeName = created.GetType().Name;
    string namespacedType = created.GetType().GetNamespaceQualifiedName();
    string data = _serializer.Serialize(created);

    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    EventEntity entity = new(EventId.NewId(), stream, DateTime.Now, typeName, namespacedType, data, actorId, isDeleted);

    Event @event = _converter.ToEvent(entity);

    Assert.Equal(entity.Id, @event.Id.Value);
    Assert.Equal(entity.Version, @event.Version);
    Assert.Equal(actorId, @event.ActorId);
    Assert.Equal(@event.OccurredOn.AsUniversalTime(), @event.OccurredOn);
    Assert.Equal(isDeleted, @event.IsDeleted);
    Assert.Equal(created, @event.Data);
  }

  [Theory(DisplayName = "ToEventEntity: it should convert the event to the correct entity.")]
  [InlineData(null, null)]
  [InlineData(null, false)]
  [InlineData("SYSTEM", true)]
  public void Given_Event_When_ToEventEntity_Then_CorrectEntity(string? actorId, bool? isDeleted)
  {
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      ActorId = actorId == null ? null : new ActorId(actorId),
      IsDeleted = isDeleted
    };

    StreamEntity stream = new(created.StreamId, typeof(User));
    EventEntity entity = _converter.ToEventEntity(created, stream);

    Assert.Equal(0, entity.EventId);
    Assert.Equal(created.Id.Value, entity.Id);
    Assert.Same(stream, entity.Stream);
    Assert.Equal(stream.StreamId, entity.StreamId);
    Assert.Equal(created.Version, entity.Version);
    Assert.Equal(actorId, entity.ActorId);
    Assert.Equal(created.OccurredOn.AsUniversalTime(), entity.OccurredOn);
    Assert.Equal(isDeleted, entity.IsDeleted);
    Assert.Equal(created.GetType().Name, entity.TypeName);
    Assert.Equal(created.GetType().GetNamespaceQualifiedName(), entity.NamespacedType);
    Assert.Equal(_serializer.Serialize(created), entity.Data);
  }

  [Fact(DisplayName = "ToEventEntity: it should convert an event without metadata to the correct entity.")]
  public void Given_NoEventMetadata_When_ToEventEntity_Then_EntityWithoutMetadata()
  {
    UserEnabled enabled = new();

    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    EventEntity entity = _converter.ToEventEntity(enabled, stream);

    Assert.Equal(0, entity.EventId);
    Assert.False(string.IsNullOrWhiteSpace(entity.Id));
    Assert.Same(stream, entity.Stream);
    Assert.Equal(stream.StreamId, entity.StreamId);
    Assert.Equal(stream.Version + 1, entity.Version);
    Assert.Null(entity.ActorId);
    Assert.Equal(DateTime.UtcNow, entity.OccurredOn, TimeSpan.FromSeconds(1));
    Assert.Null(entity.IsDeleted);
    Assert.Equal(enabled.GetType().Name, entity.TypeName);
    Assert.Equal(enabled.GetType().GetNamespaceQualifiedName(), entity.NamespacedType);
    Assert.Equal(_serializer.Serialize(enabled), entity.Data);
  }
}
