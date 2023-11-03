namespace Logitar.EventSourcing.Infrastructure;

[Trait(Traits.Category, Categories.Unit)]
public class EventSerializerTests
{
  private readonly EventSerializer _serializer = new(new[] { new CultureInfoConverter() });

  [Fact(DisplayName = "Ctor: it should construct the correct EventSerializer.")]
  public void Ctor_it_should_construct_the_correct_EventSerializer()
  {
    EventSerializer serializer = new();

    FieldInfo? optionsField = typeof(EventSerializer).GetField("_options", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.NotNull(optionsField);

    JsonSerializerOptions? options = (JsonSerializerOptions?)optionsField.GetValue(serializer);
    Assert.NotNull(options);

    Assert.Contains(options.Converters, converter => converter is ActorIdConverter);
    Assert.Contains(options.Converters, converter => converter is AggregateIdConverter);
    Assert.Contains(options.Converters, converter => converter is JsonStringEnumConverter);
  }

  [Fact(DisplayName = "Ctor: it should construct the correct EventSerializer from a list of converters.")]
  public void Ctor_it_should_construct_the_correct_EventSerializer_from_a_list_of_converters()
  {
    EventSerializer serializer = new(new[] { new CultureInfoConverter() });

    FieldInfo? optionsField = typeof(EventSerializer).GetField("_options", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.NotNull(optionsField);

    JsonSerializerOptions? options = (JsonSerializerOptions?)optionsField.GetValue(serializer);
    Assert.NotNull(options);

    Assert.Contains(options.Converters, converter => converter is ActorIdConverter);
    Assert.Contains(options.Converters, converter => converter is AggregateIdConverter);
    Assert.Contains(options.Converters, converter => converter is JsonStringEnumConverter);
    Assert.Contains(options.Converters, converter => converter is CultureInfoConverter);
  }

  [Fact(DisplayName = "Deserialize: it should deserialize the correct domain event.")]
  public void Deserialize_it_should_deserialize_the_correct_domain_event()
  {
    DefaultLanguageChangedEvent expected = new(CultureInfo.GetCultureInfo("en-CA"))
    {
      Id = Guid.NewGuid(),
      AggregateId = AggregateId.NewId(),
      Version = 5,
      ActorId = new ActorId("fpion"),
      OccurredOn = DateTime.Now
    };
    EventEntityMock entity = new()
    {
      Id = expected.Id,
      EventType = expected.GetType().GetName(),
      EventData = _serializer.Serialize(expected)
    };

    DomainEvent actual = _serializer.Deserialize(entity);
    Assert.Equal(expected, actual);
  }

  [Fact(DisplayName = "Deserialize: it should throw EventDataDeserializationFailedException when deserialized failed.")]
  public void Deserialize_it_should_throw_EventDataDeserializationFailedException_when_deserialized_failed()
  {
    EventEntityMock entity = new()
    {
      Id = Guid.NewGuid(),
      EventType = typeof(DefaultLanguageChangedEvent).GetName(),
      EventData = "null"
    };
    var exception = Assert.Throws<EventDataDeserializationFailedException>(() => _serializer.Deserialize(entity));
    Assert.Equal(entity.Id, exception.EventId);
    Assert.Equal(entity.EventType, exception.EventType);
    Assert.Equal(entity.EventData, exception.EventData);
  }

  [Fact(DisplayName = "Deserialize: it should throw EventTypeNotFoundException when event type was not found.")]
  public void Deserialize_it_should_throw_EventTypeNotFoundException_when_event_type_was_not_found()
  {
    EventEntityMock entity = new()
    {
      Id = Guid.NewGuid(),
      EventType = "Test"
    };
    var exception = Assert.Throws<EventTypeNotFoundException>(() => _serializer.Deserialize(entity));
    Assert.Equal(entity.Id, exception.EventId);
    Assert.Equal(entity.EventType, exception.TypeName);
  }

  [Fact(DisplayName = "RegisterConverter: it should register the specified converter.")]
  public void RegisterConverter_it_should_register_the_specified_converter()
  {
    EventSerializer serializer = new();

    FieldInfo? optionsField = typeof(EventSerializer).GetField("_options", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.NotNull(optionsField);

    JsonSerializerOptions? options = (JsonSerializerOptions?)optionsField.GetValue(serializer);
    Assert.NotNull(options);

    Assert.DoesNotContain(options.Converters, converter => converter is CultureInfoConverter);

    serializer.RegisterConverter(new CultureInfoConverter());
    Assert.Contains(options.Converters, converter => converter is CultureInfoConverter);
  }

  [Fact(DisplayName = "Serialize: it should serialize events correctly.")]
  public void Serialize_it_should_serialize_events_correctly()
  {
    DefaultLanguageChangedEvent e = new(CultureInfo.GetCultureInfo("en-CA"))
    {
      Id = Guid.NewGuid(),
      AggregateId = AggregateId.NewId(),
      Version = 5,
      ActorId = new ActorId("fpion"),
      OccurredOn = DateTime.Now
    };

    string expected = $@"{{""Culture"":""en-CA"",""Id"":""{e.Id}"",""AggregateId"":""{e.AggregateId}"",""Version"":5,""ActorId"":""fpion"",""OccurredOn"":{JsonSerializer.Serialize(e.OccurredOn)},""IsDeleted"":null}}";
    string actual = _serializer.Serialize(e);

    Assert.Equal(expected, actual);
  }
}
