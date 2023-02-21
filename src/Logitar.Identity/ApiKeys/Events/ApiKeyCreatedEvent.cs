using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.ApiKeys.Events;

/// <summary>
/// Represents the event raised when a new <see cref="ApiKeyAggregate"/> is created.
/// </summary>
public record ApiKeyCreatedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Gets or sets the identifier of the realm in which the API key belongs.
  /// </summary>
  public AggregateId RealmId { get; init; }

  /// <summary>
  /// Gets or sets the salted and hashed secret of the API key.
  /// </summary>
  public string SecretHash { get; init; } = string.Empty;

  /// <summary>
  /// Gets or sets the title (or display name) of the API key.
  /// </summary>
  public string Title { get; init; } = string.Empty;
  /// <summary>
  /// Gets or sets a textual description for the API key.
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Gets or sets the date and time when the API key expires.
  /// </summary>
  public DateTime? ExpiresOn { get; init; }

  /// <summary>
  /// Gets or sets the custom attributes of the API key.
  /// </summary>
  public Dictionary<string, string> CustomAttributes { get; init; } = new();

  /// <summary>
  /// Gets or sets the role (scope) identifiers of the API key.
  /// </summary>
  public IEnumerable<AggregateId> Roles { get; init; } = Enumerable.Empty<AggregateId>();
}
