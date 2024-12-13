namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a JSON converter for instances of <see cref="EventId"/> structs.
/// </summary>
public class EventIdConverter : JsonConverter<EventId>
{
  /// <summary>
  /// Reads an <see cref="EventId"/> from the specified JSON reader.
  /// </summary>
  /// <param name="reader">The JSON reader.</param>
  /// <param name="typeToConvert">The type to convert to.</param>
  /// <param name="options">The serializer options.</param>
  /// <returns>The resulting event identifier.</returns>
  public override EventId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new EventId() : new(value);
  }

  /// <summary>
  /// Writes an <see cref="EventId"/> to the specified JSON writer.
  /// </summary>
  /// <param name="writer">The JSON writer.</param>
  /// <param name="eventId">The event identifier to write.</param>
  /// <param name="options">The serializer options.</param>
  public override void Write(Utf8JsonWriter writer, EventId eventId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(eventId.Value);
  }
}
