namespace Logitar.EventSourcing.Infrastructure;

[Trait(Traits.Category, Categories.Unit)]
public class EventIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  public EventIdConverterTests()
  {
    _serializerOptions.Converters.Add(new EventIdConverter());
  }

  [Fact(DisplayName = "Read: it should read the correct instance from a string representation.")]
  public void Given_StringValue_When_Read_Then_CorrectEventId()
  {
    string value = EventId.NewId().Value;
    EventId eventId = JsonSerializer.Deserialize<EventId>($@"""{value}""", _serializerOptions);
    Assert.Equal(value, eventId.Value);
  }

  [Theory(DisplayName = "Read: it should read the default instance from a null or empty string value.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_NullValue_When_Read_Then_DefaultEventId(string? value)
  {
    value = value == null ? "null" : string.Concat('"', value, '"');
    EventId actorId = JsonSerializer.Deserialize<EventId>(value, _serializerOptions);
    Assert.True(string.IsNullOrEmpty(actorId.Value));
  }

  [Fact(DisplayName = "Write: it should write the corerct string representation.")]
  public void Given_EventId_When_Write_Then_CorrectStringValue()
  {
    EventId eventId = EventId.NewId();
    string json = JsonSerializer.Serialize(eventId, _serializerOptions);
    Assert.Equal(eventId.Value, json.Trim('"'));
  }
}
