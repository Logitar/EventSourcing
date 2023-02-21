namespace Logitar.Identity.Contacts;

/// <summary>
/// The email address update input data.
/// </summary>
public record UpdateEmailInput : UpdateContactInput
{
  /// <summary>
  /// Gets or sets the address of the email.
  /// </summary>
  public string Address { get; private set; } = string.Empty;
}
