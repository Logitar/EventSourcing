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

  [Theory(DisplayName = "Read: it should read the default instance from a null or empty string value.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_NullValue_When_Read_Then_DefaultStreamId(string? value)
  {
    value = value == null ? "null" : string.Concat('"', value, '"');
    StreamId actorId = JsonSerializer.Deserialize<StreamId>(value, _serializerOptions);
    Assert.True(string.IsNullOrEmpty(actorId.Value));
  }

  [Fact(DisplayName = "Write: it should write the corerct string representation.")]
  public void Given_StreamId_When_Write_Then_CorrectStringValue()
  {
    StreamId streamId = StreamId.NewId();
    string json = JsonSerializer.Serialize(streamId, _serializerOptions);
    Assert.Equal(streamId.Value, json.Trim('"'));
  }
}
