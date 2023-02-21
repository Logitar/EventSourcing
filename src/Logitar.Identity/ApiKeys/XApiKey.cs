using Logitar.EventSourcing;
using System.Diagnostics.CodeAnalysis;

namespace Logitar.Identity.ApiKeys;

/// <summary>
/// Represents the data structure containing data of X-API-Keys.
/// </summary>
public readonly struct XApiKey
{
  /// <summary>
  /// The character used to separate API key values.
  /// </summary>
  private const char Separator = '.';

  /// <summary>
  /// Initializes a new instance of the <see cref="XApiKey"/> struct using the specified arguments.
  /// </summary>
  /// <param name="prefix">The prefix of the API key.</param>
  /// <param name="id">The identifier of the API key.</param>
  /// <param name="secret">The secret of the API key.</param>
  public XApiKey(string prefix, Guid id, byte[] secret)
  {
    if (string.IsNullOrWhiteSpace(prefix))
    {
      throw new ArgumentException("The value cannot be null, empty or white space only.", nameof(prefix));
    }
    if (id == Guid.Empty)
    {
      throw new ArgumentException("The value cannot be empty.", nameof(id));
    }
    if (secret.Length == 0)
    {
      throw new ArgumentException("The value cannot be empty.", nameof(secret));
    }

    Prefix = prefix.ToUpper();
    Id = id;
    Secret = secret;
  }

  /// <summary>
  /// Gets the prefix of the API key.
  /// </summary>
  public string Prefix { get; }
  /// <summary>
  /// Gets the identifier of the API key.
  /// </summary>
  public Guid Id { get; }
  /// <summary>
  /// Gets the secret of the API key.
  /// </summary>
  public byte[] Secret { get; }

  /// <summary>
  /// Parses the specified string as an X-API-Key.
  /// </summary>
  /// <param name="s">The string to parse.</param>
  /// <param name="prefix">The expected prefix.</param>
  /// <returns>The parsed X-API-Key.</returns>
  public static XApiKey Parse(string s, string prefix)
  {
    string[] values = s.Split(Separator);
    if (values.Length != 3)
    {
      throw new ArgumentException($"The value '{s}' is not a valid X-API-Key.", nameof(s));
    }
    else if (!values[0].Equals(prefix, StringComparison.InvariantCulture))
    {
      throw new ArgumentException($"The X-API-Key prefix '{values[0]}' was not expected.", nameof(s));
    }

    return new XApiKey(values[0],
      new Guid(Convert.FromBase64String(values[1].FromUriSafeBase64())),
      Convert.FromBase64String(values[2].FromUriSafeBase64()));
  }
  /// <summary>
  /// Tries parsing the specified string as an X-API-Key.
  /// </summary>
  /// <param name="s">The string to parse.</param>
  /// <param name="prefix">The expected prefix.</param>
  /// <param name="xApiKey">The parsed X-API-Key.</param>
  /// <returns>A value indicating whether or not the X-API-Key could be parsed.</returns>
  public static bool TryParse(string s, string prefix, out XApiKey xApiKey)
  {
    try
    {
      xApiKey = Parse(s, prefix);

      return true;
    }
    catch (Exception)
    {
      xApiKey = default;

      return false;
    }
  }

  /// <summary>
  /// Returns a value indicating whether or not two API keys are equal.
  /// </summary>
  /// <param name="x">The first API key.</param>
  /// <param name="y">The second API key.</param>
  /// <returns>True if the API keys are equal.</returns>
  public static bool operator ==(XApiKey x, XApiKey y) => x.Equals(y);
  /// <summary>
  /// Returns a value indicating whether or not two API keys are different.
  /// </summary>
  /// <param name="x">The first API key.</param>
  /// <param name="y">The second API key.</param>
  /// <returns>True if the API keys are different</returns>
  public static bool operator !=(XApiKey x, XApiKey y) => !x.Equals(y);

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the current API key.
  /// </summary>
  /// <param name="obj">The object to compare.</param>
  /// <returns>True if the object is equal to the current API key.</returns>
  public override bool Equals([NotNullWhen(true)] object? obj) => obj is XApiKey apiKey
    && apiKey.Prefix == Prefix
    && apiKey.Id == Id
    && apiKey.Secret == Secret;
  /// <summary>
  /// Returns an integer representing the current API key hash code, derived from its values.
  /// </summary>
  /// <returns>The current API key hash code.</returns>
  public override int GetHashCode() => HashCode.Combine(Prefix, Id, Secret);
  /// <summary>
  /// Returns a string representing the current API key from its values.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => string.Join(Separator,
    Prefix,
    Convert.ToBase64String(Id.ToByteArray()).ToUriSafeBase64(),
    Convert.ToBase64String(Secret).ToUriSafeBase64());
}
