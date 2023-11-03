namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Represents a JSON converter for instances of <see cref="ActorId"/> structs.
/// </summary>
public class ActorIdConverter : JsonConverter<ActorId>
{
  /// <summary>
  /// Reads an <see cref="ActorId"/> from the specified JSON reader.
  /// </summary>
  /// <param name="reader">The JSON reader.</param>
  /// <param name="typeToConvert">The type to convert to.</param>
  /// <param name="options">The serializer options.</param>
  /// <returns>The resulting actor identifier.</returns>
  public override ActorId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? default : new ActorId(value);
  }

  /// <summary>
  /// Writes an <see cref="ActorId"/> to the specified JSON writer.
  /// </summary>
  /// <param name="writer">The JSON writer.</param>
  /// <param name="actorId">The actor identifier to write.</param>
  /// <param name="options">The serializer options.</param>
  public override void Write(Utf8JsonWriter writer, ActorId actorId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(actorId.Value);
  }
}
