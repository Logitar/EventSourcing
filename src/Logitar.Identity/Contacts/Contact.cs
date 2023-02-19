using Logitar.Identity.Users;

namespace Logitar.Identity.Contacts;

/// <summary>
/// The base output representation of contact informations.
/// </summary>
public abstract record Contact : Aggregate
{
  /// <summary>
  /// Gets or sets the user owning the contact.
  /// </summary>
  public User? User { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the contact was archived.
  /// </summary>
  public DateTime? ArchivedOn { get; set; }
  /// <summary>
  /// Gets or sets the archivation status of the contact.
  /// </summary>
  public bool IsArchived { get; set; }
  /// <summary>
  /// Gets or sets the default status of the contact.
  /// </summary>
  public bool IsDefault { get; set; }
  /// <summary>
  /// Gets or sets the date and time when the contact was verified.
  /// </summary>
  public DateTime? VerifiedOn { get; set; }
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
  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
