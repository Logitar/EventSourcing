using MediatR;

namespace Logitar.Identity.Contacts.Events;

/// <summary>
/// Represents the event raised when a new <see cref="EmailAggregate"/> is created.
/// </summary>
public record EmailCreatedEvent : ContactCreatedEvent, INotification
{
  /// <summary>
  /// Gets or sets the address of the email.
  /// </summary>
  public string Address { get; init; } = string.Empty;

  /// <summary>
  /// Gets or sets the login status of the email address. If true, the email address can be used to sign-in.
  /// </summary>
  public bool IsLogin { get; init; }
}
