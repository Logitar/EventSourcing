using Logitar.EventSourcing;
using Logitar.Identity.Contacts.Events;

namespace Logitar.Identity.Contacts;

/// <summary>
/// The base domain aggregate common to contact informations.
/// </summary>
public abstract class ContactAggregate : AggregateRoot
{
  /// <summary>
  /// The custom attributes of the contact.
  /// </summary>
  private readonly Dictionary<string, string> _customAttributes = new();

  /// <summary>
  /// Initializes a new instance of the <see cref="ContactAggregate"/> class.
  /// </summary>
  protected ContactAggregate() : base()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ContactAggregate"/> class using the specified aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  protected ContactAggregate(AggregateId id) : base(id)
  {
  }

  /// <summary>
  /// Gets the identifier of the user owning the contact.
  /// </summary>
  public AggregateId UserId { get; private set; }

  /// <summary>
  /// Gets the archivation status of the contact.
  /// </summary>
  public bool IsArchived { get; private set; }
  /// <summary>
  /// Gets the default status of the contact.
  /// </summary>
  public bool IsDefault { get; private set; }
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
  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  /// <summary>
  /// Applies the specified event to the contact.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(ContactCreatedEvent e)
  {
    UserId = e.UserId;

    IsArchived = e.IsArchived;
    IsDefault = e.IsDefault;
    IsVerified = e.IsVerified;

    Label = e.Label;

    _customAttributes.Clear();
    _customAttributes.AddRange(e.CustomAttributes);
  }

  /// <summary>
  /// Applies the specified event to the contact.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(ContactSetDefaultEvent e)
  {
    IsDefault = e.IsDefault;
  }

  /// <summary>
  /// Applies the specified event to the contact.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(ContactUpdatedEvent e)
  {
    IsArchived = e.IsArchived;
    IsDefault = e.IsDefault;

    switch (e.VerificationAction)
    {
      case VerificationAction.Unverify:
        IsVerified = false;
        break;
      case VerificationAction.Verify:
        IsVerified = true;
        break;
    }

    Label = e.Label;

    _customAttributes.Clear();
    _customAttributes.AddRange(e.CustomAttributes);
  }

  /// <summary>
  /// Applies the specified event to the contact.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected virtual void Apply(ContactVerifiedEvent e)
  {
    IsVerified = e.IsVerified;
  }
}
