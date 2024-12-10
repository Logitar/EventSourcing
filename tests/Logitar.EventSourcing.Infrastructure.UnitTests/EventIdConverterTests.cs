using System.Text.Json;

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

  [Fact(DisplayName = "Read: it should read the default instance from a null string value.")]
  public void Given_NullValue_When_Read_Then_DefaultEventId()
  {
    EventId eventId = JsonSerializer.Deserialize<EventId>("null", _serializerOptions);
    Assert.True(string.IsNullOrEmpty(eventId.Value));
  }

  [Fact(DisplayName = "Write: it should write the corerct string representation.")]
  public void Given_EventId_When_Write_Then_CorrectStringValue()
  {
    EventId eventId = EventId.NewId();
    string json = JsonSerializer.Serialize(eventId, _serializerOptions);
    Assert.Equal(eventId.Value, json.Trim('"'));
  }
}
