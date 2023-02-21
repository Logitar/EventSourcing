namespace Logitar.Identity.Contacts;

/// <summary>
/// The output representation of a email address.
/// </summary>
public record Email : Contact
{
  /// <summary>
  /// Gets or sets or sets the identifier of the email address.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the address of the email.
  /// </summary>
  public string Address { get; private set; } = string.Empty;
}
