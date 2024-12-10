using System.Text.Json;

namespace Logitar.EventSourcing.Infrastructure;

[Trait(Traits.Category, Categories.Unit)]
public class StreamIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  public StreamIdConverterTests()
  {
    _serializerOptions.Converters.Add(new StreamIdConverter());
  }

  [Fact(DisplayName = "Read: it should read the correct instance from a string representation.")]
  public void Given_StringValue_When_Read_Then_CorrectStreamId()
  {
    string value = StreamId.NewId().Value;
    StreamId streamId = JsonSerializer.Deserialize<StreamId>($@"""{value}""", _serializerOptions);
    Assert.Equal(value, streamId.Value);
  }

  [Fact(DisplayName = "Read: it should read the default instance from a null string value.")]
  public void Given_NullValue_When_Read_Then_DefaultStreamId()
  {
    StreamId streamId = JsonSerializer.Deserialize<StreamId>("null", _serializerOptions);
    Assert.True(string.IsNullOrEmpty(streamId.Value));
  }

  [Fact(DisplayName = "Write: it should write the corerct string representation.")]
  public void Given_StreamId_When_Write_Then_CorrectStringValue()
  {
    StreamId streamId = StreamId.NewId();
    string json = JsonSerializer.Serialize(streamId, _serializerOptions);
    Assert.Equal(streamId.Value, json.Trim('"'));
  }
}
