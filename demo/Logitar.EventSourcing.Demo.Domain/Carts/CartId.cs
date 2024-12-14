namespace Logitar.EventSourcing.Demo.Domain.Carts;

public readonly struct CartId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public CartId(string value)
  {
    StreamId = new StreamId(value);
  }
  public CartId(Guid value)
  {
    StreamId = new StreamId(value);
  }
  public CartId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static CartId NewId() => new(Guid.NewGuid());
  public Guid ToGuid() => new(Convert.FromBase64String(Value.FromUriSafeBase64()));

  public static bool operator ==(CartId left, CartId right) => left.Equals(right);
  public static bool operator !=(CartId left, CartId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is CartId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
