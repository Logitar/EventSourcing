using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.JsonConverters;

/// <summary>
/// The converter used to serialize and deserialize instance of the <see cref="CultureInfo"/> class.
/// </summary>
public class CultureInfoConverter : JsonConverter<CultureInfo?>
{
  /// <summary>
  /// Reads an instance of the <see cref="CultureInfo"/> class from the specified JSON reader.
  /// </summary>
  /// <param name="reader">The JSON reader.</param>
  /// <param name="typeToConvert">The type of the instance; it should be <see cref="CultureInfo"/>.</param>
  /// <param name="options">The serialization options.</param>
  /// <returns>The instance of the <see cref="CultureInfo"/> class.</returns>
  public override CultureInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? name = reader.GetString();

    return name == null ? null : CultureInfo.GetCultureInfo(name);
  }

  /// <summary>
  /// Writes an instance of the <see cref="CultureInfo"/> class to the specified JSON writer.
  /// </summary>
  /// <param name="writer">The JSON writer.</param>
  /// <param name="value">The type of the instance; it should be <see cref="CultureInfo"/>.</param>
  /// <param name="options">The serialization options.</param>
  public override void Write(Utf8JsonWriter writer, CultureInfo? value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value?.Name);
  }
}
