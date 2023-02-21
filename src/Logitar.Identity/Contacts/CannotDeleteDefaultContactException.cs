namespace Logitar.Identity.Contacts;

/// <summary>
/// The exception thrown when a default contact is targeted by deletion. A default contact can only
/// be deleted if it is the only contact of its type for its user.
/// </summary>
public class CannotDeleteDefaultContactException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="CannotDeleteDefaultContactException"/> class using the specified arguments.
  /// </summary>
  /// <param name="contact">The contact targeted by deletion.</param>
  public CannotDeleteDefaultContactException(ContactAggregate contact)
    : base($"The default contact '{contact}' cannot be deleted, unless it is the only contact of its type of the user.")
  {
    Data["Contact"] = contact.ToString();
  }
}
