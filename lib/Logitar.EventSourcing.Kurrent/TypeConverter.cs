namespace Logitar.EventSourcing.Kurrent;

public class TypeConverter : JsonConverter<Type>
{
  public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return value == null ? null : Type.GetType(value);
  }

  public override void Write(Utf8JsonWriter writer, Type type, JsonSerializerOptions options)
  {
    writer.WriteStringValue(type.GetNamespaceQualifiedName());
  }
}
