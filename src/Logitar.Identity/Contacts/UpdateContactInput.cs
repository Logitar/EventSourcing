namespace Logitar.Identity.Contacts;

/// <summary>
/// The base contact information update input data.
/// </summary>
public abstract record UpdateContactInput
{
  /// <summary>
  /// Gets or sets the archivation status of the contact.
  /// </summary>
  public bool IsArchived { get; set; }
  /// <summary>
  /// Gets or sets the default status action of the contact. If true, the contact will be set default.
  /// </summary>
  public bool SetDefault { get; set; }
  /// <summary>
  /// Gets or sets the verification status of the contact.
  /// </summary>
  public bool IsVerified { get; set; }

  /// <summary>
  /// Gets or sets the label describing the contact.
  /// </summary>
  public string? Label { get; set; }

  /// <summary>
  /// Gets or sets the custom attributes of the contact.
  /// </summary>
  public IEnumerable<CustomAttribute>? CustomAttributes { get; set; }
}
