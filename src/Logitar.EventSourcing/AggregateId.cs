using System.Diagnostics.CodeAnalysis;

namespace Logitar.EventSourcing;

public readonly struct AggregateId
{
  public AggregateId(Guid value) : this(Convert.ToBase64String(value.ToByteArray()).ToUriSafeBase64())
  {
  }
  public AggregateId(string value)
  {
    Value = value.CleanTrim() ?? throw new ArgumentException("The value cannot be null, empty or only white spaces.", nameof(value));
  }

  public string Value { get; }

  public static AggregateId NewId() => new(Guid.NewGuid());

  public static bool operator ==(AggregateId x, AggregateId y) => x.Equals(y);
  public static bool operator !=(AggregateId x, AggregateId y) => !x.Equals(y);

  public Guid ToGuid() => new(Convert.FromBase64String(Value.FromUriSafeBase64()));

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is AggregateId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
