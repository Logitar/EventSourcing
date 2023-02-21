using Logitar.EventSourcing;
using Logitar.Identity.Contacts.Events;
using System.Text.Json;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model representing a contact.
/// </summary>
internal abstract class ContactEntity : AggregateEntity, ICustomAttributes
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ContactEntity"/> class.
  /// </summary>
  protected ContactEntity() : base()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="ContactEntity"/> to the state of the specified event.
  /// </summary>
  /// <param name="e">The creation event.</param>
  /// <param name="user">The user owning the contact.</param>
  protected ContactEntity(ContactCreatedEvent e, UserEntity user) : base(e)
  {
    User = user;
    UserId = user.UserId;

    SetArchivation(e, e.IsArchived);
    IsDefault = e.IsDefault;
    SetVerification(e, e.IsVerified);

    Label = e.Label;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }

  /// <summary>
  /// Gets the user owning the contact.
  /// </summary>
  public UserEntity? User { get; private set; }
  /// <summary>
  /// Gets the identifier of the user owning the contact.
  /// </summary>
  public int UserId { get; private set; }

  /// <summary>
  /// Gets the identifier of the actor who archived the contact.
  /// </summary>
  public string? ArchivedById { get; private set; }
  /// <summary>
  /// Gets the date and time when the contact was archived.
  /// </summary>
  public DateTime? ArchivedOn { get; private set; }
  /// <summary>
  /// Gets the archivation status of the contact.
  /// </summary>
  public bool IsArchived { get; private set; }
  /// <summary>
  /// Gets the default status of the contact.
  /// </summary>
  public bool IsDefault { get; private set; }
  /// <summary>
  /// Gets the identifier of the actor who verified the contact.
  /// </summary>
  public string? VerifiedById { get; private set; }
  /// <summary>
  /// Gets the date and time when the contact was verified.
  /// </summary>
  public DateTime? VerifiedOn { get; private set; }
  /// <summary>
  /// Gets the verification status of the contact.
  /// </summary>
  public bool IsVerified { get; private set; }

  /// <summary>
  /// Gets the label describing the contact.
  /// </summary>
  public string? Label { get; private set; }

  /// <summary>
  /// Gets the custom attributes of the contact.
  /// </summary>
  public string? CustomAttributes { get; private set; }

  /// <summary>
  /// Sets the default status of the contact to the state of the specified event.
  /// </summary>
  /// <param name="e">The set default event.</param>
  protected void SetDefault(ContactSetDefaultEvent e)
  {
    Update(e);

    IsDefault = e.IsDefault;
  }

  /// <summary>
  /// Updates the contact to the state of the specified event.
  /// </summary>
  /// <param name="e">The update event.</param>
  protected void Update(ContactUpdatedEvent e)
  {
    base.Update(e);

    SetArchivation(e, e.IsArchived);
    IsDefault = e.IsDefault;

    switch (e.VerificationAction)
    {
      case VerificationAction.Unverify:
        SetVerification(e, isVerified: false);
        break;
      case VerificationAction.Verify:
        SetVerification(e, isVerified: true);
        break;
    }

    Label = e.Label;

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }

  /// <summary>
  /// Sets the verification status of the contact to the state of the specified event.
  /// </summary>
  /// <param name="e">The verification event.</param>
  protected void Verify(ContactVerifiedEvent e)
  {
    SetVersion(e);

    SetVerification(e, e.IsVerified);
  }

  /// <summary>
  /// Sets the archivation status of the contact to the state of the specified event.
  /// </summary>
  /// <param name="e">The archivation event.</param>
  /// <param name="isArchived">A value indicating whether or not the contact is archived.</param>
  private void SetArchivation(DomainEvent e, bool isArchived)
  {
    if (isArchived)
    {
      ArchivedById = e.ActorId.Value;
      ArchivedOn = e.OccurredOn;
    }
    else
    {
      ArchivedById = null;
      ArchivedOn = null;
    }

    IsArchived = isArchived;
  }

  /// <summary>
  /// Sets the verification status of the contact to the state of the specified event.
  /// </summary>
  /// <param name="e">The verification event.</param>
  /// <param name="isVerified">A value indicating whether or not the contact is verified.</param>
  private void SetVerification(DomainEvent e, bool isVerified)
  {
    if (isVerified)
    {
      VerifiedById = e.ActorId.Value;
      VerifiedOn = e.OccurredOn;
    }
    else
    {
      VerifiedById = null;
      VerifiedOn = null;
    }

    IsVerified = isVerified;
  }
}
