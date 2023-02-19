namespace Logitar.Identity.Contacts;

/// <summary>
/// The email address creation input data.
/// </summary>
public record CreateEmailInput : CreateContactInput
{
  /// <summary>
  /// Gets or sets the address of the email.
  /// </summary>
  public string Address { get; private set; } = string.Empty;
}
