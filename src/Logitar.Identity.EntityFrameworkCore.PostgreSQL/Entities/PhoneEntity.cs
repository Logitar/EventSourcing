using Logitar.Identity.Contacts.Events;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model representing an phone number.
/// </summary>
internal class PhoneEntity : ContactEntity
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneEntity"/> to the state of the specified event.
  /// </summary>
  /// <param name="e">The creation event.</param>
  /// <param name="user">The user owning the phone number.</param>
  public PhoneEntity(PhoneCreatedEvent e, UserEntity user) : base(e, user)
  {
    CountryCode = e.CountryCode;
    Number = e.Number;
    Extension = e.Extension;
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneEntity"/> class.
  /// </summary>
  private PhoneEntity() : base()
  {
  }

  /// <summary>
  /// Gets the primary identifier of the phone number.
  /// </summary>
  public int PhoneId { get; private set; }

  /// <summary>
  /// Gets the country code of the phone.
  /// </summary>
  public string? CountryCode { get; private set; }

  /// <summary>
  /// Gets the number of the phone.
  /// </summary>
  public string Number { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the extension of the phone.
  /// </summary>
  public string? Extension { get; private set; }

  /// <summary>
  /// Sets the default status of the phone number to the state of the specified event.
  /// </summary>
  /// <param name="e">The set default event.</param>
  public void SetDefault(PhoneSetDefaultEvent e)
  {
    base.SetDefault(e);
  }

  /// <summary>
  /// Updates the phone number to the state of the specified event.
  /// </summary>
  /// <param name="e">The update event.</param>
  public void Update(PhoneUpdatedEvent e)
  {
    base.Update(e);

    CountryCode = e.CountryCode;
    Number = e.Number;
    Extension = e.Extension;
  }

  /// <summary>
  /// Sets the verification status of the phone number to the state of the specified event.
  /// </summary>
  /// <param name="e">The verification event.</param>
  public void Verify(PhoneVerifiedEvent e)
  {
    base.Verify(e);
  }
}
