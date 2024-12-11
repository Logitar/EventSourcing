using Bogus;
using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

[Trait(Traits.Category, Categories.Unit)]
public class EventConverterTests
{
  private readonly Faker _faker = new();
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly EventSerializer _serializer;
  private readonly FakeEventConverter _converter;

  private readonly string _streamIdValue = StreamId.NewId().Value;
  private readonly Uuid _eventUuid = Uuid.NewUuid();
  private readonly StreamPosition _eventNumber = StreamPosition.FromInt64(0);
  private readonly Position _position = Position.Start;
  private readonly Dictionary<string, string> _metadata = new()
  {
    ["type"] = typeof(UserCreated).Name,
    ["created"] = DateTime.UtcNow.Ticks.ToString(),
    ["content-type"] = MediaTypeNames.Application.Json
  };

  private readonly UserCreated _event;
  private readonly ReadOnlyMemory<byte> _data;

  private readonly EventMetadata _eventMetadata;
  private readonly ReadOnlyMemory<byte> _customMetadata;

  public EventConverterTests()
  {
    _serializerOptions.Converters.Add(new ActorIdConverter());
    _serializerOptions.Converters.Add(new EventIdConverter());
    _serializerOptions.Converters.Add(new TypeConverter());

    _serializer = new EventSerializer();
    _converter = new FakeEventConverter(_serializer);

    _event = new UserCreated(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      ActorId = ActorId.NewId(),
      IsDeleted = false
    };
    _data = Encoding.UTF8.GetBytes(_serializer.Serialize(_event));

    _eventMetadata = new EventMetadata(_event.GetType(), _event.Id, typeof(User), _event.Version, _event.ActorId, _event.OccurredOn, _event.IsDeleted);
    _customMetadata = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_eventMetadata, _serializerOptions));
  }

  [Fact(DisplayName = "GetEventMetadata: it should return null when the event has no metadata.")]
  public void Given_NoEventMetadata_When_GetEventMetadata_Then_Null()
  {
    EventRecord record = new(_streamIdValue, _eventUuid, _eventNumber, _position, _metadata, _data, customMetadata: new ReadOnlyMemory<byte>());
    Assert.Null(_converter.GetEventMetadata<EventMetadata>(record));
  }

  [Theory(DisplayName = "GetEventMetadata: it should return the correct metadata.")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_EventMetadata_When_GetEventMetadata_Then_CorrectMetadata(bool isNull)
  {
    ReadOnlyMemory<byte> customMetadata = isNull ? Encoding.UTF8.GetBytes("null") : _customMetadata;
    EventRecord record = new(_streamIdValue, _eventUuid, _eventNumber, _position, _metadata, _data, customMetadata);

    EventMetadata? metadata = _converter.GetEventMetadata<EventMetadata>(record);
    if (isNull)
    {
      Assert.Null(metadata);
    }
    else
    {
      Assert.NotNull(metadata);
      Assert.Equal(_eventMetadata, metadata);
    }
  }

  [Fact(DisplayName = "GetEventType: it should return the correct event type from event metadata.")]
  public void Given_EventMetadata_When_GetEventType_Then_CorrectType()
  {
    EventRecord record = new(_streamIdValue, _eventUuid, _eventNumber, _position, _metadata, _data, customMetadata: new ReadOnlyMemory<byte>());
    Type eventType = _converter.GetEventTypeExposed(record, _eventMetadata);
    Assert.Equal(_event.GetType(), eventType);
  }

  [Fact(DisplayName = "GetEventType: it should throw EventTypeNotFoundException when the event type could not be resolved.")]
  public void Given_EventTypeNotResolved_When_GetEventType_Then_EventTypeNotFoundException()
  {
    _metadata["type"] = typeof(UserSignedIn).Name;

    EventRecord record = new(_streamIdValue, _eventUuid, _eventNumber, _position, _metadata, _data, customMetadata: new ReadOnlyMemory<byte>());

    var exception = Assert.Throws<EventTypeNotFoundException>(() => _converter.GetEventTypeExposed(record, metadata: null));
    Assert.Equal(record.EventId.ToString(), exception.EventId);
    Assert.Equal(record.EventType, exception.TypeName);
  }

  [Fact(DisplayName = "GetEventType: it should try resolving the event type from the event record when there is no metadata.")]
  public void Given_NoEventMetadata_When_GetEventType_Then_ResolveFromEventType()
  {
    _metadata["type"] = _event.GetType().GetNamespaceQualifiedName();

    EventRecord record = new(_streamIdValue, _eventUuid, _eventNumber, _position, _metadata, _data, customMetadata: new ReadOnlyMemory<byte>());
    Type eventType = _converter.GetEventTypeExposed(record, _eventMetadata);
    Assert.Equal(_event.GetType(), eventType);
  }

  [Fact(DisplayName = "GetStreamType: it should return null when the event has no metadata.")]
  public void Given_NoEventMetadata_When_GetStreamType_Then_Null()
  {
    EventRecord record = new(_streamIdValue, _eventUuid, _eventNumber, _position, _metadata, _data, customMetadata: new ReadOnlyMemory<byte>());
    Assert.Null(_converter.GetStreamType(record));
  }

  [Fact(DisplayName = "GetStreamType: it should return the stream type from the event metadata.")]
  public void Given_EventMetadata_When_GetStreamType_Then_FromMetadata()
  {
    EventRecord record = new(_streamIdValue, _eventUuid, _eventNumber, _position, _metadata, _data, _customMetadata);
    Type? streamType = _converter.GetStreamType(record);
    Assert.NotNull(streamType);
    Assert.Equal(typeof(User), streamType);
  }

  [Fact(DisplayName = "ToEvent: it should return the correct event from metadata.")]
  public void Given_EventMetadata_When_ToEvent_Then_CorrectEvent()
  {
    EventRecord record = new(_streamIdValue, _eventUuid, _eventNumber, _position, _metadata, _data, _customMetadata);

    Event @event = _converter.ToEvent(record);

    Assert.Equal(_eventMetadata.EventId, @event.Id);
    Assert.Equal(_eventMetadata.Version, @event.Version);
    Assert.Equal(_eventMetadata.ActorId, @event.ActorId);
    Assert.Equal(_eventMetadata.OccurredOn, @event.OccurredOn);
    Assert.Equal(_eventMetadata.IsDeleted, @event.IsDeleted);
    Assert.Equal(_event, @event.Data);
  }

  [Fact(DisplayName = "ToEvent: it should return the correct event without metadata.")]
  public void Given_NoEventMetadata_When_ToEvent_Then_CorrectEvent()
  {
    EventRecord record = new(_streamIdValue, _eventUuid, _eventNumber, _position, _metadata, _data, customMetadata: new ReadOnlyMemory<byte>());

    Event @event = _converter.ToEvent(record);

    Assert.Equal(new EventId(_eventUuid.ToGuid()), @event.Id);
    Assert.Equal(_eventNumber.ToInt64() + 1, @event.Version);
    Assert.Null(@event.ActorId);
    Assert.Equal(record.Created, @event.OccurredOn);
    Assert.Null(@event.IsDeleted);
    Assert.Equal(_event, @event.Data);
  }

  [Theory(DisplayName = "ToEventData: it should convert the event and stream type to the correct EventData.")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_EventAndStreamType_When_ToEventData_Then_CorrectEventData(bool hasStreamType)
  {
    Type? streamType = hasStreamType ? typeof(User) : null;

    EventData eventData = _converter.ToEventData(_event, streamType);

    Assert.Equal(_event.Id.ToGuid(), eventData.EventId.ToGuid());
    Assert.Equal(_event.GetType().Name, eventData.Type);
    Assert.Equal(_data, eventData.Data);
    Assert.Equal(MediaTypeNames.Application.Json, eventData.ContentType);

    if (hasStreamType)
    {
      Assert.Equal(_customMetadata, eventData.Metadata);
    }
    else
    {
      EventMetadata eventMetadata = new(_eventMetadata.EventType, _eventMetadata.EventId, streamType: null, _eventMetadata.Version, _eventMetadata.ActorId, _eventMetadata.OccurredOn, _eventMetadata.IsDeleted);
      string json = JsonSerializer.Serialize(eventMetadata, _serializerOptions);
      ReadOnlyMemory<byte> metadata = Encoding.UTF8.GetBytes(json);
      Assert.Equal(metadata, eventData.Metadata);
    }
  }

  [Fact(DisplayName = "ToEventData: it should convert the event with minimal metadata.")]
  public void Given_MinimalMetadata_When_ToEventData_Then_CorrectEventData()
  {
    UserEnabled @event = new();

    EventData eventData = _converter.ToEventData(@event, streamType: null);

    Assert.NotEqual(Guid.Empty, eventData.EventId.ToGuid());
    Assert.Equal(@event.GetType().Name, eventData.Type);
    Assert.Equal(Encoding.UTF8.GetBytes("{}"), eventData.Data);
    Assert.Equal(MediaTypeNames.Application.Json, eventData.ContentType);

    EventMetadata eventMetadata = new(@event.GetType());
    string json = JsonSerializer.Serialize(eventMetadata, _serializerOptions);
    ReadOnlyMemory<byte> metadata = Encoding.UTF8.GetBytes(json);
    Assert.Equal(metadata, eventData.Metadata);
  }

  [Fact(DisplayName = "ToEventData: it should generate a new UUID when the event ID is not a Guid.")]
  public void Given_IdNotGuid_When_ToEventData_Then_NewUuidGenerated()
  {
    UserCreated @event = new(_faker.Person.UserName)
    {
      Id = new EventId(_faker.Internet.UserName()),
      StreamId = StreamId.NewId(),
      Version = 1
    };

    EventData eventData = _converter.ToEventData(@event, typeof(User));

    Assert.NotEqual(Guid.Empty, eventData.EventId.ToGuid());

    string json = Encoding.UTF8.GetString(eventData.Metadata.ToArray());
    EventMetadata? metadata = JsonSerializer.Deserialize<EventMetadata>(json, _serializerOptions);
    Assert.NotNull(metadata);
    Assert.Equal(@event.Id, metadata.EventId);
  }
}
