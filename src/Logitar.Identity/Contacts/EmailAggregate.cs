using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contacts.Events;
using Logitar.Identity.Contacts.Validators;
using Logitar.Identity.Users;

namespace Logitar.Identity.Contacts;

/// <summary>
/// The domain aggregate representing an email address. Email addresses belong to an user. If an user
/// has email addresses, a single email address must be designated as the default. When allowed, they
/// may be used to sign-in.
/// </summary>
public class EmailAggregate : ContactAggregate
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EmailAggregate"/> class using the specified aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  public EmailAggregate(AggregateId id) : base(id)
  {
  }

  /// <summary>
  /// Initializes a new intance of the <see cref="EmailAggregate"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorId">The identifier of the actor creating the email address.</param>
  /// <param name="user">The user owning the email address.</param>
  /// <param name="address">The address of the email.</param>
  /// <param name="isLogin">The login status of the email address. If true, the email address may be used to sign-in.</param>
  /// <param name="isArchived">The archivation status of the email address.</param>
  /// <param name="isDefault">The default status of the email address.</param>
  /// <param name="isVerified">The verification status of the email address.</param>
  /// <param name="label">The label describing the email address.</param>
  /// <param name="customAttributes">The custom attributes of the email address.</param>
  public EmailAggregate(AggregateId actorId, UserAggregate user, string address, bool isLogin = false,
    bool isArchived = false, bool isDefault = false, bool isVerified = false, string? label = null,
    Dictionary<string, string>? customAttributes = null) : base()
  {
    EmailCreatedEvent e = new()
    {
      ActorId = actorId,
      UserId = user.Id,
      IsArchived = isArchived,
      IsDefault = isDefault,
      IsVerified = isVerified,
      Label = label?.CleanTrim(),
      Address = address.Trim(),
      IsLogin = isLogin,
      CustomAttributes = customAttributes ?? new()
    };
    new EmailCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

  /// <summary>
  /// Gets the address of the email.
  /// </summary>
  public string Address { get; private set; } = string.Empty;

  /// <summary>
  /// Gets the login status of the email address. If true, the email address may be used to sign-in.
  /// </summary>
  public bool IsLogin { get; private set; }

  /// <summary>
  /// Applies the specified event to the email address.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(EmailCreatedEvent e)
  {
    base.Apply(e);

    Address = e.Address;

    IsLogin = e.IsLogin;
  }

  /// <summary>
  /// Deletes the email address.
  /// </summary>
  /// <param name="actorId">The identifier of the actor deleting the email address.</param>
  public void Delete(AggregateId actorId)
  {
    ApplyChange(new EmailDeletedEvent
    {
      ActorId = actorId
    });
  }
  /// <summary>
  /// Applies the specified event to the email address.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(EmailDeletedEvent e)
  {
  }

  /// <summary>
  /// Sets the default status of the email address.
  /// </summary>
  /// <param name="actorId">The identifier of the actor setting the default status of the email address.</param>
  /// <param name="isDefault">The default status of the email address.</param>
  public void SetDefault(AggregateId actorId, bool isDefault = true)
  {
    ApplyChange(new EmailSetDefaultEvent()
    {
      ActorId = actorId,
      IsDefault = isDefault
    });
  }
  /// <summary>
  /// Applies the specified event to the email address.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(EmailSetDefaultEvent e)
  {
    base.Apply(e);
  }

  /// <summary>
  /// Updates the email address.
  /// </summary>
  /// <param name="actorId">The identifier of the actor updating the email address.</param>
  /// <param name="address">The address of the email. If changed, the email address may be unverified.</param>
  /// <param name="isLogin">The login status of the email address. If true, the email address may be used to sign-in.</param>
  /// <param name="isArchived">The archivation status of the email address.</param>
  /// <param name="isDefault">The default status of the email address.</param>
  /// <param name="isVerified">The verification status of the email address.</param>
  /// <param name="label">The label describing the email address.</param>
  /// <param name="customAttributes">The custom attributes of the email address.</param>
  public void Update(AggregateId actorId, string address, bool isLogin, bool isArchived,
    bool isDefault, bool isVerified, string? label, Dictionary<string, string>? customAttributes)
  {
    address = address.Trim();

    VerificationAction verificationAction = VerificationAction.None;
    if (isVerified)
    {
      verificationAction = VerificationAction.Verify;
    }
    else if (Address != address)
    {
      verificationAction = VerificationAction.Unverify;
    }

    EmailUpdatedEvent e = new()
    {
      ActorId = actorId,
      IsArchived = isArchived,
      IsDefault = isDefault,
      VerificationAction = verificationAction,
      Label = label?.CleanTrim(),
      Address = address,
      IsLogin = isLogin,
      CustomAttributes = customAttributes ?? new()
    };
    new EmailUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  /// <summary>
  /// Applies the specified event to the email address.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(EmailUpdatedEvent e)
  {
    base.Apply(e);

    Address = e.Address;

    IsLogin = e.IsLogin;
  }

  /// <summary>
  /// Sets the verification status of the email address.
  /// </summary>
  /// <param name="actorId">The identifier of the actor setting the verification status of the email address.</param>
  /// <param name="isVerified">The verification status of the email address.</param>
  public void Verify(AggregateId actorId, bool isVerified = true)
  {
    ApplyChange(new EmailVerifiedEvent()
    {
      ActorId = actorId,
      IsVerified = isVerified
    });
  }
  /// <summary>
  /// Applies the specified event to the email address.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(EmailVerifiedEvent e)
  {
    base.Apply(e);
  }

  /// <summary>
  /// Returns a string representation of the current email address.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => $"{Address} | {base.ToString()}";
}
