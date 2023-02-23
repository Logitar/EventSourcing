namespace Logitar.EventSourcing;

/// <summary>
/// Represents the unique identifier of a domain aggregate.
/// </summary>
public readonly struct AggregateId
{
  /// <summary>
  /// The value of the aggregate identifier.
  /// </summary>
  private readonly string _value;

  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateId"/> struct.
  /// </summary>
  [Obsolete("To be removed in the next major version, since it masks the default implicit constructor, and does not work as intended.")]
  public AggregateId()
  {
    _value = string.Empty;
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateId"/> struct with the specified <see cref="Guid"/>.
  /// To optimize storage and comparisons, the Guid bytes will be encoded in base64.
  /// </summary>
  /// <param name="value">The Guid value.</param>
  public AggregateId(Guid value) : this(Convert.ToBase64String(value.ToByteArray()).ToUriSafeBase64())
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateId"/> struct with the specified arguments.
  /// </summary>
  /// <param name="value">The string value.</param>
  /// <exception cref="ArgumentException">The string is null, empty or only white spaces.</exception>
  public AggregateId(string value)
  {
    value = value.CleanTrim() ?? throw new ArgumentException("The value cannot be null, empty or only white spaces.", nameof(value));

    if (value.Length > byte.MaxValue)
    {
      throw new ArgumentException($"The value cannot exceed {byte.MaxValue} characters.", nameof(value));
    }

    _value = value;
  }

  /// <summary>
  /// Gets the value of the aggregate identifier.
  /// </summary>
  public string Value => _value ?? string.Empty;

  /// <summary>
  /// Creates a new <see cref="AggregateId"/> using a random <see cref="Guid"/>.
  /// </summary>
  /// <returns>The newly aggregate identifier.</returns>
  public static AggregateId NewId() => new(Guid.NewGuid());

  /// <summary>
  /// Returns a value indicating whether or not two aggregate identifiers are equal.
  /// </summary>
  /// <param name="x">The first aggregate identifier.</param>
  /// <param name="y">The second aggregate identifier.</param>
  /// <returns>True if the aggregate identifiers are equal.</returns>
  public static bool operator ==(AggregateId x, AggregateId y) => x.Equals(y);
  /// <summary>
  /// Returns a value indicating whether or not two aggregate identifiers are different.
  /// </summary>
  /// <param name="x">The first aggregate identifier.</param>
  /// <param name="y">The second aggregate identifier.</param>
  /// <returns>True if the aggregate identifiers are different.</returns>
  public static bool operator !=(AggregateId x, AggregateId y) => !x.Equals(y);

  /// <summary>
  /// Retrieves the <see cref="Guid"/> encoded in the current aggregate identifier value.
  /// </summary>
  /// <returns>The corresponding Guid.</returns>
  public Guid ToGuid() => new(Convert.FromBase64String(Value.FromUriSafeBase64()));

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the current aggregate identifier.
  /// </summary>
  /// <param name="obj">The object to compare.</param>
  /// <returns>True if the object is equal to the current aggregate identifier.</returns>
  public override bool Equals([NotNullWhen(true)] object? obj) => obj is AggregateId id && id.Value == Value;
  /// <summary>
  /// Returns an integer representing the current aggregate identifier hash code, derived from its value.
  /// </summary>
  /// <returns>The current aggregate identifier hash code.</returns>
  public override int GetHashCode() => Value.GetHashCode();
  /// <summary>
  /// Returns a string representing the current aggregate identifier; its value.
  /// </summary>
  /// <returns>The current aggregate identifier value.</returns>
  public override string ToString() => Value;
}
