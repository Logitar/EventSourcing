namespace Logitar.EventSourcing;

/// <summary>
/// Represents a domain event, typically attached to a domain aggregate.
/// </summary>
public abstract record DomainEvent
{
  /// <summary>
  /// The event unique identifier
  /// </summary>
  public Guid Id { get; set; } = Guid.NewGuid();

  /// <summary>
  /// The identifier of the aggregate owning the event
  /// </summary>
  public AggregateId AggregateId { get; set; }
  /// <summary>
  /// The version of the event
  /// </summary>
  public long Version { get; set; }

  /// <summary>
  /// The identifier of the actor who triggered the event
  /// </summary>
  public AggregateId ActorId { get; set; } = new AggregateId("SYSTEM");
  /// <summary>
  /// The date and time when the event happened
  /// </summary>
  public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

  /// <summary>
  /// The delete action performed by this event
  /// </summary>
  public DeleteAction DeleteAction { get; set; }
}
