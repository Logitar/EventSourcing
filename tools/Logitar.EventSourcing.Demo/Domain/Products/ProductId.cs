using System.Diagnostics.CodeAnalysis;

namespace Logitar.EventSourcing.Demo.Domain.Products;

public readonly struct ProductId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public ProductId(string value)
  {
    StreamId = new StreamId(value);
  }
  public ProductId(Guid value)
  {
    StreamId = new StreamId(value);
  }
  public ProductId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static ProductId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => new(Convert.FromBase64String(Value.FromUriSafeBase64()));

  public static bool operator ==(ProductId left, ProductId right) => left.Equals(right);
  public static bool operator !=(ProductId left, ProductId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ProductId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
