using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Relational;

/// <summary>
/// Represents the relational storage model for events.
/// </summary>
public class EventEntity : IEventEntity
{
  /// <summary>
  /// Gets of sets the primary key of the event.
  /// </summary>
  public long EventId { get; set; }
  /// <summary>
  /// Gets or sets the identifier of the event.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who triggered the event.
  /// </summary>
  public string ActorId { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets a value indicating whether or not the aggregate is deleted.
  /// </summary>
  public bool? IsDeleted { get; set; }
  /// <summary>
  /// Gets or sets the date and time when the event occurred.
  /// </summary>
  public DateTime OccurredOn { get; set; }
  /// <summary>
  /// Gets or sets the version of the event.
  /// </summary>
  public long Version { get; set; }

  /// <summary>
  /// Gets or sets the type of the aggregate to which the event belongs to.
  /// </summary>
  public string AggregateType { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the identifier of the aggregate to which the event belongs to.
  /// </summary>
  public string AggregateId { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the type of the event.
  /// </summary>
  public string EventType { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the data of the event.
  /// </summary>
  public string EventData { get; set; } = string.Empty;
}
