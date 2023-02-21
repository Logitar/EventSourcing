using Logitar.Identity.Contacts.Events;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model representing a postal address.
/// </summary>
internal class AddressEntity : ContactEntity
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AddressEntity"/> to the state of the specified event.
  /// </summary>
  /// <param name="e">The creation event.</param>
  /// <param name="user">The user owning the postal address.</param>
  public AddressEntity(AddressCreatedEvent e, UserEntity user) : base(e, user)
  {
    Line1 = e.Line1;
    Line2 = e.Line2;
    Locality = e.Locality;
    PostalCode = e.PostalCode;
    Country = e.Country;
    Region = e.Region;
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="AddressEntity"/> class.
  /// </summary>
  private AddressEntity() : base()
  {
  }

  /// <summary>
  /// Gets the primary identifier of the postal address.
  /// </summary>
  public int AddressId { get; private set; }

  /// <summary>
  /// Gets the primary line of the postal address.
  /// </summary>
  public string Line1 { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the secondary line of the postal address.
  /// </summary>
  public string? Line2 { get; private set; }

  /// <summary>
  /// Gets the locality (or city) of the postal address.
  /// </summary>
  public string Locality { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the postal code of the postal address.
  /// </summary>
  public string? PostalCode { get; private set; }

  /// <summary>
  /// Gets the country of the postal address.
  /// </summary>
  public string Country { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the region of the postal address.
  /// </summary>
  public string? Region { get; private set; }

  /// <summary>
  /// Sets the default status of the postal address to the state of the specified event.
  /// </summary>
  /// <param name="e">The set default event.</param>
  public void SetDefault(AddressSetDefaultEvent e)
  {
    base.SetDefault(e);
  }

  /// <summary>
  /// Updates the postal address to the state of the specified event.
  /// </summary>
  /// <param name="e">The update event.</param>
  public void Update(AddressUpdatedEvent e)
  {
    base.Update(e);

    Line1 = e.Line1;
    Line2 = e.Line2;
    Locality = e.Locality;
    PostalCode = e.PostalCode;
    Country = e.Country;
    Region = e.Region;
  }

  /// <summary>
  /// Sets the verification status of the postal address to the state of the specified event.
  /// </summary>
  /// <param name="e">The verification event.</param>
  public void Verify(AddressVerifiedEvent e)
  {
    base.Verify(e);
  }
}
