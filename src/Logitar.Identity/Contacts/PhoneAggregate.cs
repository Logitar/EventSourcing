using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contacts.Events;
using Logitar.Identity.Contacts.Validators;
using Logitar.Identity.Users;
using PhoneNumbers;
using System.Text;

namespace Logitar.Identity.Contacts;

/// <summary>
/// The domain aggregate representing a phone number. Phones belong to an user. If an user has phone
/// numbers, a single phone number must be designated as the default.
/// </summary>
public class PhoneAggregate : ContactAggregate
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneAggregate"/> class using the specified aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  public PhoneAggregate(AggregateId id) : base(id)
  {
  }

  /// <summary>
  /// Initializes a new intance of the <see cref="PhoneAggregate"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorId">The identifier of the actor creating the phone number.</param>
  /// <param name="user">The user owning the phone number.</param>
  /// <param name="countryCode">The country code of the phone.</param>
  /// <param name="number">The number of the phone.</param>
  /// <param name="extension">The extension of the phone.</param>
  /// <param name="isArchived">The archivation status of the phone number.</param>
  /// <param name="isDefault">The default status of the phone number.</param>
  /// <param name="isVerified">The verification status of the phone number.</param>
  /// <param name="label">The label describing the phone number.</param>
  /// <param name="customAttributes">The custom attributes of the phone number.</param>
  public PhoneAggregate(AggregateId actorId, UserAggregate user, string countryCode, string number,
    string? extension = null, bool isArchived = false, bool isDefault = false, bool isVerified = false,
    string? label = null, Dictionary<string, string>? customAttributes = null) : base()
  {
    PhoneCreatedEvent e = new()
    {
      ActorId = actorId,
      UserId = user.Id,
      IsArchived = isArchived,
      IsDefault = isDefault,
      IsVerified = isVerified,
      Label = label?.CleanTrim(),
      CountryCode = countryCode.Trim(),
      Number = number.Trim(),
      Extension = extension?.CleanTrim(),
      CustomAttributes = customAttributes ?? new()
    };
    new PhoneCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

  /// <summary>
  /// Gets the country code of the phone.
  /// </summary>
  public string CountryCode { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the number of the phone.
  /// </summary>
  public string Number { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the extension of the phone.
  /// </summary>
  public string? Extension { get; private set; }

  /// <summary>
  /// Applies the specified event to the phone number.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(PhoneCreatedEvent e)
  {
    base.Apply(e);

    CountryCode = e.CountryCode;
    Number = e.Number;
    Extension = e.Extension;
  }

  /// <summary>
  /// Deletes the phone number.
  /// </summary>
  /// <param name="actorId">The identifier of the actor deleting the phone number.</param>
  public void Delete(AggregateId actorId)
  {
    ApplyChange(new PhoneDeletedEvent
    {
      ActorId = actorId
    });
  }
  /// <summary>
  /// Applies the specified event to the phone number.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(PhoneDeletedEvent e)
  {
  }

  /// <summary>
  /// Sets the default status of the phone number.
  /// </summary>
  /// <param name="actorId">The identifier of the actor setting the default status of the phone number.</param>
  /// <param name="isDefault">The default status of the phone number.</param>
  public void SetDefault(AggregateId actorId, bool isDefault = true)
  {
    ApplyChange(new PhoneSetDefaultEvent()
    {
      ActorId = actorId,
      IsDefault = isDefault
    });
  }
  /// <summary>
  /// Applies the specified event to the phone number.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(PhoneSetDefaultEvent e)
  {
    base.Apply(e);
  }

  /// <summary>
  /// Updates the phone number.
  /// </summary>
  /// <param name="actorId">The identifier of the actor updating the phone number.</param>
  /// <param name="countryCode">The country code of the phone.</param>
  /// <param name="number">The number of the phone.</param>
  /// <param name="extension">The extension of the phone.</param>
  /// <param name="isArchived">The archivation status of the phone number.</param>
  /// <param name="isDefault">The default status of the phone number.</param>
  /// <param name="isVerified">The verification status of the phone number.</param>
  /// <param name="label">The label describing the phone number.</param>
  /// <param name="customAttributes">The custom attributes of the phone number.</param>
  public void Update(AggregateId actorId, string countryCode, string number,
    string? extension = null, bool isArchived = false, bool isDefault = false, bool isVerified = false,
    string? label = null, Dictionary<string, string>? customAttributes = null)
  {
    PhoneUpdatedEvent e = new()
    {
      ActorId = actorId,
      IsArchived = isArchived,
      IsDefault = isDefault,
      IsVerified = isVerified,
      Label = label?.CleanTrim(),
      CountryCode = countryCode.Trim(),
      Number = number.Trim(),
      Extension = extension?.CleanTrim(),
      CustomAttributes = customAttributes ?? new()
    };
    new PhoneUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  /// <summary>
  /// Applies the specified event to the phone number.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(PhoneUpdatedEvent e)
  {
    base.Apply(e);

    CountryCode = e.CountryCode;
    Number = e.Number;
    Extension = e.Extension;
  }

  /// <summary>
  /// Sets the verification status of the phone number.
  /// </summary>
  /// <param name="actorId">The identifier of the actor setting the verification status of the phone number.</param>
  /// <param name="isVerified">The verification status of the phone number.</param>
  public void Verify(AggregateId actorId, bool isVerified = true)
  {
    ApplyChange(new PhoneVerifiedEvent()
    {
      ActorId = actorId,
      IsVerified = isVerified
    });
  }
  /// <summary>
  /// Applies the specified event to the phone number.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(PhoneVerifiedEvent e)
  {
    base.Apply(e);
  }

  /// <summary>
  /// Returns a string representation of the current phone number.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString()
  {
    StringBuilder s = new();

    s.Append(CountryCode);
    s.Append(' ');
    s.Append(Number);

    if (Extension != null)
    {
      s.Append(" x");
      s.Append(Extension);
    }

    PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
    PhoneNumber phone = phoneUtil.Parse(s.ToString(), defaultRegion: string.Empty);
    string formatted = phoneUtil.Format(phone, PhoneNumberFormat.INTERNATIONAL);

    return $"{formatted} | {base.ToString()}";
  }
}
