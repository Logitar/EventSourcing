namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a JSON converter for instances of <see cref="StreamId"/> structs.
/// </summary>
public class StreamIdConverter : JsonConverter<StreamId>
{
  /// <summary>
  /// Reads an <see cref="StreamId"/> from the specified JSON reader.
  /// </summary>
  /// <param name="reader">The JSON reader.</param>
  /// <param name="typeToConvert">The type to convert to.</param>
  /// <param name="options">The serializer options.</param>
  /// <returns>The resulting stream identifier.</returns>
  public override StreamId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? default : new StreamId(value);
  }

  /// <summary>
  /// Writes an <see cref="StreamId"/> to the specified JSON writer.
  /// </summary>
  /// <param name="writer">The JSON writer.</param>
  /// <param name="streamId">The stream identifier to write.</param>
  /// <param name="options">The serializer options.</param>
  public override void Write(Utf8JsonWriter writer, StreamId streamId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(streamId.Value);
  }
}
