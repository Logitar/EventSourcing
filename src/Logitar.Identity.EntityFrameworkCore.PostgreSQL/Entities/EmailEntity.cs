using Logitar.Identity.Contacts.Events;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model representing an email address.
/// </summary>
internal class EmailEntity : ContactEntity
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EmailEntity"/> to the state of the specified event.
  /// </summary>
  /// <param name="e">The creation event.</param>
  /// <param name="user">The user owning the email address.</param>
  public EmailEntity(EmailCreatedEvent e, UserEntity user) : base(e, user)
  {
    Address = e.Address;
    IsLogin = e.IsLogin;
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="EmailEntity"/> class.
  /// </summary>
  private EmailEntity() : base()
  {
  }

  /// <summary>
  /// Gets the primary identifier of the email address.
  /// </summary>
  public int EmailId { get; private set; }

  /// <summary>
  /// Gets the address of the email.
  /// </summary>
  public string Address { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the login status of the email address. If true, the email address may be used to sign-in.
  /// </summary>
  public bool IsLogin { get; private set; }

  /// <summary>
  /// Sets the default status of the email address to the state of the specified event.
  /// </summary>
  /// <param name="e">The set default event.</param>
  public void SetDefault(EmailSetDefaultEvent e)
  {
    base.SetDefault(e);
  }

  /// <summary>
  /// Updates the email address to the state of the specified event.
  /// </summary>
  /// <param name="e">The update event.</param>
  public void Update(EmailUpdatedEvent e)
  {
    base.Update(e);

    Address = e.Address;
    IsLogin = e.IsLogin;
  }

  /// <summary>
  /// Sets the verification status of the email address to the state of the specified event.
  /// </summary>
  /// <param name="e">The verification event.</param>
  public void Verify(EmailVerifiedEvent e)
  {
    base.Verify(e);
  }
}
