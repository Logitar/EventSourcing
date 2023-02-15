using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Roles.Events;

/// <summary>
/// Represents the event raised when a new <see cref="RoleAggregate"/> is created.
/// </summary>
public record RoleCreatedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Gets or sets the identifier of the realm in which the role belongs.
  /// </summary>
  public AggregateId RealmId { get; init; }

  /// <summary>
  /// Gets or sets the unique name of the role (not case-sensitive).
  /// </summary>
  public string UniqueName { get; init; } = string.Empty;
  /// <summary>
  /// Gets or sets the display name of the role.
  /// </summary>
  public string? DisplayName { get; init; }
  /// <summary>
  /// Gets or sets a textual description for the role.
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Gets or sets the custom attributes of the role.
  /// </summary>
  public Dictionary<string, string> CustomAttributes { get; init; } = new();
}
