namespace Logitar.EventSourcing.Infrastructure;

[Trait(Traits.Category, Categories.Unit)]
public class ActorIdConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  public ActorIdConverterTests()
  {
    _serializerOptions.Converters.Add(new ActorIdConverter());
  }

  [Fact(DisplayName = "Read: it should read the correct instance from a string representation.")]
  public void Given_StringValue_When_Read_Then_CorrectActorId()
  {
    string value = ActorId.NewId().Value;
    ActorId actorId = JsonSerializer.Deserialize<ActorId>($@"""{value}""", _serializerOptions);
    Assert.Equal(value, actorId.Value);
  }

  [Theory(DisplayName = "Read: it should read the default instance from a null or empty string value.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Given_NullValue_When_Read_Then_DefaultActorId(string? value)
  {
    value = value == null ? "null" : string.Concat('"', value, '"');
    ActorId actorId = JsonSerializer.Deserialize<ActorId>(value, _serializerOptions);
    Assert.True(string.IsNullOrEmpty(actorId.Value));
  }

  [Fact(DisplayName = "Write: it should write the corerct string representation.")]
  public void Given_ActorId_When_Write_Then_CorrectStringValue()
  {
    ActorId actorId = ActorId.NewId();
    string json = JsonSerializer.Serialize(actorId, _serializerOptions);
    Assert.Equal(actorId.Value, json.Trim('"'));
  }
}
