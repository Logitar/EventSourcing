using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when an <see cref="EmailAggregate"/> is updated.
/// </summary>
public record EmailUpdatedEvent : ContactUpdatedEvent, INotification
{
  /// <summary>
  /// Gets or sets the address of the email.
  /// </summary>
  public string Address { get; init; } = string.Empty;

  /// <summary>
  /// Gets or sets the login status of the email address. If true, the email address may be used to sign-in.
  /// </summary>
  public bool IsLogin { get; init; }
}
