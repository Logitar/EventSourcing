namespace Logitar.EventSourcing.Infrastructure;

internal class CultureInfoConverter : JsonConverter<CultureInfo?>
{
  public override CultureInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? name = reader.GetString();

    return name == null ? null : CultureInfo.GetCultureInfo(name);
  }

  public override void Write(Utf8JsonWriter writer, CultureInfo? value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value?.Name);
  }
}
