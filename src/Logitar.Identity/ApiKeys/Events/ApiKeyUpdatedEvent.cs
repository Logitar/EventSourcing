using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.ApiKeys.Events;

/// <summary>
/// Represents the event raised when an <see cref="ApiKeyAggregate"/> is updated.
/// </summary>
public record ApiKeyUpdatedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Gets or sets the title (or display name) of the API key.
  /// </summary>
  public string Title { get; init; } = string.Empty;
  /// <summary>
  /// Gets or sets a textual description for the API key.
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Gets or sets the custom attributes of the API key.
  /// </summary>
  public Dictionary<string, string> CustomAttributes { get; init; } = new();

  /// <summary>
  /// Gets or sets the role (scope) identifiers of the API key.
  /// </summary>
  public IEnumerable<AggregateId> Roles { get; init; } = Enumerable.Empty<AggregateId>();
}
