namespace Logitar.EventSourcing;

/// <summary>
/// Represents the identifier of an actor.
/// </summary>
public readonly struct ActorId
{
  /// <summary>
  /// The default value of the identifier.
  /// </summary>
  public const string DefaultValue = "SYSTEM";
  /// <summary>
  /// The maximum length of the identifier value.
  /// </summary>
  public const int MaximumLength = byte.MaxValue;

  /// <summary>
  /// The value of the identifier.
  /// </summary>
  private readonly string? _value = null;

  /// <summary>
  /// Initializes a new instance of the <see cref="ActorId"/> struct.
  /// </summary>
  /// <param name="value">The value of the identifier.</param>
  public ActorId(Guid value) : this(Convert.ToBase64String(value.ToByteArray()).ToUriSafeBase64())
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="ActorId"/> struct.
  /// </summary>
  /// <param name="value">The value of the identifier.</param>
  /// <exception cref="ArgumentException">The value is null, empty or only white space.</exception>
  /// <exception cref="ArgumentOutOfRangeException">The value exceeds the maximum length.</exception>
  public ActorId(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("The value is required.", nameof(value));
    }

    value = value.Trim();
    if (value.Length > MaximumLength)
    {
      throw new ArgumentOutOfRangeException(nameof(value), $"The value may contain up to {MaximumLength} characters.");
    }

    _value = value;
  }

  /// <summary>
  /// Gets the value of the identifier.
  /// </summary>
  public string Value => _value ?? DefaultValue;

  /// <summary>
  /// Returns a value indicating whether or not the specified identifiers are equal.
  /// </summary>
  /// <param name="left">The first identifier to compare.</param>
  /// <param name="right">The other identifier to compare.</param>
  /// <returns>True if the identifiers are equal.</returns>
  public static bool operator ==(ActorId left, ActorId right) => left.Equals(right);
  /// <summary>
  /// Returns a value indicating whether or not the specified identifiers are different.
  /// </summary>
  /// <param name="left">The first identifier to compare.</param>
  /// <param name="right">The other identifier to compare.</param>
  /// <returns>True if the identifiers are different.</returns>
  public static bool operator !=(ActorId left, ActorId right) => !left.Equals(right);

  /// <summary>
  /// Creates a new instance of the <see cref="ActorId"/> struct from a random <see cref="Guid"/>.
  /// </summary>
  /// <returns>The created instance.</returns>
  public static ActorId NewId() => new(Guid.NewGuid());
  /// <summary>
  /// Converts the identifier to a <see cref="Guid"/>. The conversion will fail if the identifier has not been created from a <see cref="Guid"/>.
  /// </summary>
  /// <returns>The resulting Guid.</returns>
  public Guid ToGuid() => new(Convert.FromBase64String(Value.FromUriSafeBase64()));

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the identifier.
  /// </summary>
  /// <param name="obj">The object to be compared to.</param>
  /// <returns>True if the object is equal to the identifier.</returns>
  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ActorId id && id.Value == Value;
  /// <summary>
  /// Returns the hash code of the current identifier.
  /// </summary>
  /// <returns>The hash code.</returns>
  public override int GetHashCode() => Value.GetHashCode();
  /// <summary>
  /// Returns a string representation of the identifier.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => Value;
}
