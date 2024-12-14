using Logitar.EventSourcing.Demo.Domain.Products;

namespace Logitar.EventSourcing.Demo.Infrastructure.Converters;

internal class ProductIdConverter : JsonConverter<ProductId>
{
  public override ProductId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new ProductId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, ProductId productId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(productId.Value);
  }
}
