using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contacts.Events;
using Logitar.Identity.Contacts.Validators;
using Logitar.Identity.Users;
using System.Text;

namespace Logitar.Identity.Contacts;

/// <summary>
/// The domain aggregate representing a postal address. Postal addresses belong to an user. If an user
/// has postal addresses, a single postal address must be designated as the default.
/// </summary>
public class AddressAggregate : ContactAggregate
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AddressAggregate"/> class using the specified aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  public AddressAggregate(AggregateId id) : base(id)
  {
  }

  /// <summary>
  /// Initializes a new intance of the <see cref="AddressAggregate"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorId">The identifier of the actor creating the postal address.</param>
  /// <param name="user">The user owning the postal address.</param>
  /// <param name="line1">The primary line of the postal address.</param>
  /// <param name="line2">The secondary line of the postal address.</param>
  /// <param name="locality">The locality (or city) of the postal address.</param>
  /// <param name="postalCode">The postal code of the postal address.</param>
  /// <param name="country">The country of the postal address.</param>
  /// <param name="region">The region of the postal address.</param>
  /// <param name="isArchived">The archivation status of the postal address.</param>
  /// <param name="isDefault">The default status of the postal address.</param>
  /// <param name="isVerified">The verification status of the postal address.</param>
  /// <param name="label">The label describing the postal address.</param>
  /// <param name="customAttributes">The custom attributes of the postal address.</param>
  public AddressAggregate(AggregateId actorId, UserAggregate user, string line1, string locality,
    string country, string? line2 = null, string? postalCode = null, string? region = null,
    bool isArchived = false, bool isDefault = false, bool isVerified = false, string? label = null,
    Dictionary<string, string>? customAttributes = null) : base()
  {
    AddressCreatedEvent e = new()
    {
      ActorId = actorId,
      UserId = user.Id,
      IsArchived = isArchived,
      IsDefault = isDefault,
      IsVerified = isVerified,
      Label = label?.CleanTrim(),
      Line1 = line1.Trim(),
      Line2 = line2?.CleanTrim(),
      Locality = locality.Trim(),
      PostalCode = postalCode?.CleanTrim(),
      Country = country.Trim(),
      Region = region?.CleanTrim(),
      CustomAttributes = customAttributes ?? new()
    };
    new AddressCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

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
  /// Applies the specified event to the postal address.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(AddressCreatedEvent e)
  {
    base.Apply(e);

    Line1 = e.Line1;
    Line2 = e.Line2;
    Locality = e.Locality;
    PostalCode = e.PostalCode;
    Country = e.Country;
    Region = e.Region;
  }

  /// <summary>
  /// Deletes the postal address.
  /// </summary>
  /// <param name="actorId">The identifier of the actor deleting the postal address.</param>
  public void Delete(AggregateId actorId)
  {
    ApplyChange(new AddressDeletedEvent
    {
      ActorId = actorId
    });
  }
  /// <summary>
  /// Applies the specified event to the postal address.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(AddressDeletedEvent e)
  {
  }

  /// <summary>
  /// Sets the default status of the postal address.
  /// </summary>
  /// <param name="actorId">The identifier of the actor setting the default status of the postal address.</param>
  /// <param name="isDefault">The default status of the postal address.</param>
  public void SetDefault(AggregateId actorId, bool isDefault = true)
  {
    ApplyChange(new AddressSetDefaultEvent()
    {
      ActorId = actorId,
      IsDefault = isDefault
    });
  }
  /// <summary>
  /// Applies the specified event to the postal address.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(AddressSetDefaultEvent e)
  {
    base.Apply(e);
  }

  /// <summary>
  /// Updates the postal address.
  /// </summary>
  /// <param name="actorId">The identifier of the actor updating the postal address.</param>
  /// <param name="line1">The primary line of the postal address.</param>
  /// <param name="line2">The secondary line of the postal address.</param>
  /// <param name="locality">The locality (or city) of the postal address.</param>
  /// <param name="postalCode">The postal code of the postal address.</param>
  /// <param name="country">The country of the postal address.</param>
  /// <param name="region">The region of the postal address.</param>
  /// <param name="isArchived">The archivation status of the postal address.</param>
  /// <param name="isDefault">The default status of the postal address.</param>
  /// <param name="isVerified">The verification status of the postal address.</param>
  /// <param name="label">The label describing the postal address.</param>
  /// <param name="customAttributes">The custom attributes of the postal address.</param>
  public void Update(AggregateId actorId, string line1, string locality, string country,
    string? line2 = null, string? postalCode = null, string? region = null, bool isArchived = false,
    bool isDefault = false, bool isVerified = false, string? label = null,
    Dictionary<string, string>? customAttributes = null)
  {
    AddressUpdatedEvent e = new()
    {
      ActorId = actorId,
      IsArchived = isArchived,
      IsDefault = isDefault,
      IsVerified = isVerified,
      Label = label?.CleanTrim(),
      Line1 = line1.Trim(),
      Line2 = line2?.CleanTrim(),
      Locality = locality.Trim(),
      PostalCode = postalCode?.CleanTrim(),
      Country = country.Trim(),
      Region = region?.CleanTrim(),
      CustomAttributes = customAttributes ?? new()
    };
    new AddressUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  /// <summary>
  /// Applies the specified event to the postal address.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(AddressUpdatedEvent e)
  {
    base.Apply(e);

    Line1 = e.Line1;
    Line2 = e.Line2;
    Locality = e.Locality;
    PostalCode = e.PostalCode;
    Country = e.Country;
    Region = e.Region;
  }

  /// <summary>
  /// Sets the verification status of the postal address.
  /// </summary>
  /// <param name="actorId">The identifier of the actor setting the verification status of the postal address.</param>
  /// <param name="isVerified">The verification status of the postal address.</param>
  public void Verify(AggregateId actorId, bool isVerified = true)
  {
    ApplyChange(new AddressVerifiedEvent()
    {
      ActorId = actorId,
      IsVerified = isVerified
    });
  }
  /// <summary>
  /// Applies the specified event to the postal address.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(AddressVerifiedEvent e)
  {
    base.Apply(e);
  }

  /// <summary>
  /// Returns a string representation of the current postal address.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString()
  {
    StringBuilder address = new();

    address.AppendLine(Line1);

    if (Line2 != null)
    {
      address.AppendLine(Line2);
    }

    address.Append(Locality);

    if (Region != null)
    {
      address.Append(' ');
      address.Append(Region);
    }
    if (PostalCode != null)
    {
      address.Append(' ');
      address.Append(PostalCode);
    }

    address.AppendLine();
    address.AppendLine(Country);

    address.Append(base.ToString());

    return address.ToString();
  }
}
