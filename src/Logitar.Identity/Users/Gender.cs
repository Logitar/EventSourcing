using System.Diagnostics.CodeAnalysis;

namespace Logitar.Identity.Users;

/// <summary>
/// Represents the gender of an user.
/// </summary>
public readonly struct Gender
{
  /// <summary>
  /// Initializes a new instance of the <see cref="Gender"/> struct with the specified string representation.
  /// </summary>
  /// <param name="value">The gender string representation.</param>
  /// <exception cref="ArgumentException">The string is null, empty or only white spaces.</exception>
  public Gender(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("The value cannot be null, empty or only white spaces.", nameof(value));
    }

    Value = value.Trim().ToLower() switch
    {
      "female" => Female.Value,
      "male" => Male.Value,
      "other" => Other.Value,
      _ => value,
    };
  }

  /// <summary>
  /// Gets the default female gender.
  /// </summary>
  public static Gender Female { get; } = new("Female");
  /// <summary>
  /// Gets the default male gender.
  /// </summary>
  public static Gender Male { get; } = new("Male");
  /// <summary>
  /// Gets the default other gender.
  /// </summary>
  public static Gender Other { get; } = new("Other");

  /// <summary>
  /// Gets the value of the gender.
  /// </summary>
  public string Value { get; }

  /// <summary>
  /// Returns a value indicating whether or not two genders are equal.
  /// </summary>
  /// <param name="x">The first gender.</param>
  /// <param name="y">The second gender.</param>
  /// <returns>True if the genders are equal.</returns>
  public static bool operator ==(Gender x, Gender y) => x.Equals(y);
  /// <summary>
  /// Returns a value indicating whether or not two genders are different.
  /// </summary>
  /// <param name="x">The first gender.</param>
  /// <param name="y">The second gender.</param>
  /// <returns>True if the genders are different.</returns>
  public static bool operator !=(Gender x, Gender y) => !x.Equals(y);

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the current gender.
  /// </summary>
  /// <param name="obj">The object to compare.</param>
  /// <returns>True if the object is equal to the current gender.</returns>
  public override bool Equals([NotNullWhen(true)] object? obj) => obj is Gender id && id.Value.ToLower() == Value.ToLower();
  /// <summary>
  /// Returns an integer representing the current gender hash code, derived from its value.
  /// </summary>
  /// <returns>The current gender hash code.</returns>
  public override int GetHashCode() => Value.GetHashCode();
  /// <summary>
  /// Returns a string representing the current gender; its value.
  /// </summary>
  /// <returns>The current gender value.</returns>
  public override string ToString() => Value;
}
