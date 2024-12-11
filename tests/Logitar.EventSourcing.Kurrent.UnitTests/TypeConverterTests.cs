using System.Text.Json;

namespace Logitar.EventSourcing.Kurrent;

[Trait(Traits.Category, Categories.Unit)]
public class TypeConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  public TypeConverterTests()
  {
    _serializerOptions.Converters.Add(new TypeConverter());
  }

  [Fact(DisplayName = "Read: it should read the correct instance from a string representation.")]
  public void Given_StringValue_When_Read_Then_CorrectType()
  {
    string value = typeof(User).GetNamespaceQualifiedName();
    Type? type = JsonSerializer.Deserialize<Type>($@"""{value}""", _serializerOptions);
    Assert.NotNull(type);
    Assert.Equal(value, type.GetNamespaceQualifiedName());
  }

  [Fact(DisplayName = "Read: it should read the default instance from a null string value.")]
  public void Given_NullValue_When_Read_Then_DefaultType()
  {
    Type? type = JsonSerializer.Deserialize<Type>("null", _serializerOptions);
    Assert.Null(type);
  }

  [Fact(DisplayName = "Write: it should write the corerct string representation.")]
  public void Given_Type_When_Write_Then_CorrectStringValue()
  {
    Type type = typeof(User);
    string json = JsonSerializer.Serialize(type, _serializerOptions);
    Assert.Equal(type.GetNamespaceQualifiedName(), json.Trim('"'));
  }
}
