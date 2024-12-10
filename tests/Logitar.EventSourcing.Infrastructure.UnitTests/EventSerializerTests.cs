using Bogus;
using System.Globalization;
using System.Text.Json;

namespace Logitar.EventSourcing.Infrastructure;

[Trait(Traits.Category, Categories.Unit)]
public class EventSerializerTests
{
  private readonly Faker _faker = new();

  private readonly EventSerializer _serializer = new();

  [Fact(DisplayName = "ctor: it should register all converters when passing SerializerOptions.")]
  public void Given_SerializerOptions_When_ctor_Then_AllConvertersRegistered()
  {
    JsonSerializerOptions serializerOptions = new();
    serializerOptions.Converters.Add(new CultureInfoConverter());
    EventSerializer serializer = new(serializerOptions);

    UserLocaleChanged change = new(CultureInfo.GetCultureInfo("fr-CA"));
    string json = serializer.Serialize(change);
    Assert.Equal($@"{{""Locale"":""{change.Locale}""}}", json);

    UserLocaleChanged? deserialized = serializer.Deserialize(typeof(UserLocaleChanged), json) as UserLocaleChanged;
    Assert.NotNull(deserialized);
    Assert.Equal(change.Locale, deserialized.Locale);
  }

  [Fact(DisplayName = "Deserialize: it should deserialize an event correctly.")]
  public void Given_TypeAndJson_When_Deserialize_Then_DeserializedCorrectly()
  {
    Gender gender = _faker.PickRandom(Enum.GetValues<Gender>());
    EventId id = EventId.NewId();
    StreamId streamId = StreamId.NewId();
    long version = 2;
    ActorId actorId = ActorId.NewId();
    DateTime occurredOn = DateTime.UtcNow;
    string json = $"{{{string.Join(',',
      $@"""Gender"":""{gender}""",
      $@"""Id"":""{id}""",
      $@"""StreamId"":""{streamId}""",
      $@"""Version"":{version}",
      $@"""ActorId"":""{actorId}""",
      $@"""OccurredOn"":""{occurredOn.ToISOString()}""",
      $@"""IsDeleted"":null")}}}";

    UserGenderChanged? change = _serializer.Deserialize(typeof(UserGenderChanged), json) as UserGenderChanged;
    Assert.NotNull(change);
    Assert.Equal(gender, change.Gender);
    Assert.Equal(id, change.Id);
    Assert.Equal(streamId, change.StreamId);
    Assert.Equal(version, change.Version);
    Assert.Equal(actorId, change.ActorId);
    Assert.Equal(occurredOn, change.OccurredOn);
    Assert.Null(change.IsDeleted);
  }

  [Fact(DisplayName = "Deserialize: it should throw EventDeserializationFailedException when deserialization returned null.")]
  public void Given_Null_When_Deserialize_Then_EventDeserializationFailedException()
  {
    Type type = typeof(UserGenderChanged);
    string json = "null";
    var exception = Assert.Throws<EventDeserializationFailedException>(() => _serializer.Deserialize(type, json));
    Assert.Equal(type.GetNamespaceQualifiedName(), exception.Type);
    Assert.Equal(json, exception.Value);
  }

  [Fact(DisplayName = "Serialize: it should serialize an event correctly.")]
  public void Given_Event_When_Serialize_Then_SerializedCorrectly()
  {
    Gender gender = _faker.PickRandom(Enum.GetValues<Gender>());
    UserGenderChanged change = new(gender)
    {
      StreamId = StreamId.NewId(),
      Version = 2,
      ActorId = ActorId.NewId(),
      OccurredOn = DateTime.UtcNow
    };

    string json = _serializer.Serialize(change);
    Assert.Equal($"{{{string.Join(',',
      $@"""Gender"":""{gender}""",
      $@"""Id"":""{change.Id}""",
      $@"""StreamId"":""{change.StreamId}""",
      $@"""Version"":2",
      $@"""ActorId"":""{change.ActorId}""",
      $@"""OccurredOn"":""{change.OccurredOn.ToISOString()}""",
      $@"""IsDeleted"":null")}}}", json);
  }
}
