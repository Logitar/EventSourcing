namespace Logitar.Identity.Contacts;

/// <summary>
/// Exposes methods to help validate postal addresses.
/// </summary>
internal static class PostalAddressHelper
{
  /// <summary>
  /// The dictionary of country validation settings.
  /// </summary>
  private static readonly Dictionary<string, CountrySettings> _countries = new();

  /// <summary>
  /// Initializes the static <see cref="PostalAddressHelper"/> class.
  /// </summary>
  static PostalAddressHelper()
  {
    _countries["CA"] = new CountrySettings
    {
      PostalCode = "[ABCEGHJ-NPRSTVXY]\\d[ABCEGHJ-NPRSTV-Z][ -]?\\d[ABCEGHJ-NPRSTV-Z]\\d$",
      Regions = new HashSet<string>(new[] { "AB", "BC", "MB", "NB", "NL", "NT", "NS", "NU", "ON", "PE", "QC", "SK", "YT" })
    };
  }

  /// <summary>
  /// Gets the list of supported countries.
  /// </summary>
  public static IEnumerable<string> SupportedCountries => _countries.Keys;

  /// <summary>
  /// Retrieves the validation settings of the specified country.
  /// </summary>
  /// <param name="country">The country.</param>
  /// <returns>The validation settings if found, or null otherwise.</returns>
  public static CountrySettings? GetCountry(string country)
  {
    return _countries.TryGetValue(country, out CountrySettings? settings) ? settings : null;
  }
}
