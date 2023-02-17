using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Users.Events;

/// <summary>
/// Represents the event raised when an external identifier is saved to an <see cref="UserAggregate"/>.
/// </summary>
public record ExternalIdentifierSavedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Gets or sets the key of the external identifier.
  /// </summary>
  public string Key { get; init; } = string.Empty;

  /// <summary>
  /// Gets or sets the value of the external identifier. If null, the external identifier will be removed.
  /// </summary>
  public string? Value { get; init; } = string.Empty;
}
