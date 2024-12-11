namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Represents a JSON converter for instances of <see cref="Type"/> classes.
/// </summary>
public class TypeConverter : JsonConverter<Type>
{
  /// <summary>
  /// Reads an <see cref="Type"/> from the specified JSON reader.
  /// </summary>
  /// <param name="reader">The JSON reader.</param>
  /// <param name="typeToConvert">The type to convert to.</param>
  /// <param name="options">The serializer options.</param>
  /// <returns>The resulting type.</returns>
  public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return value == null ? null : Type.GetType(value);
  }

  /// <summary>
  /// Writes an <see cref="Type"/> to the specified JSON writer.
  /// </summary>
  /// <param name="writer">The JSON writer.</param>
  /// <param name="type">The type to write.</param>
  /// <param name="options">The serializer options.</param>
  public override void Write(Utf8JsonWriter writer, Type type, JsonSerializerOptions options)
  {
    writer.WriteStringValue(type.GetNamespaceQualifiedName());
  }
}
