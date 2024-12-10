using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.EventSourcing.Infrastructure;

public class CultureInfoConverter : JsonConverter<CultureInfo>
{
  public override CultureInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return value == null ? null : CultureInfo.GetCultureInfo(value);
  }

  public override void Write(Utf8JsonWriter writer, CultureInfo culture, JsonSerializerOptions options)
  {
    writer.WriteStringValue(culture.Name);
  }
}
