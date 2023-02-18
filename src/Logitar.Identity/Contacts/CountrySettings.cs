namespace Logitar.Identity.Contacts;

/// <summary>
/// The settings used to validate postal addresses.
/// </summary>
internal record CountrySettings
{
  /// <summary>
  /// Gets or sets the pattern (or regular expression) used to validate postal codes of this country.
  /// </summary>
  public string? PostalCode { get; init; }
  /// <summary>
  /// Gets or sets the list of valid regions in this country.
  /// </summary>
  public HashSet<string>? Regions { get; init; }
}
