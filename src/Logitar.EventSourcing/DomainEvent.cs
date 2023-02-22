namespace Logitar.EventSourcing;

/// <summary>
/// Represents a domain event, typically attached to a domain aggregate.
/// </summary>
public abstract record DomainEvent
{
  /// <summary>
  /// Gets or sets the unique identifier of the event.
  /// </summary>
  public Guid Id { get; set; } = Guid.NewGuid();

  /// <summary>
  /// Gets or sets the identifier of the aggregate owning the event.
  /// </summary>
  public AggregateId AggregateId { get; set; }
  /// <summary>
  /// Gets or sets the version of the event.
  /// </summary>
  public long Version { get; set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who triggered the event.
  /// </summary>
  public AggregateId ActorId { get; set; } = new AggregateId("SYSTEM");
  /// <summary>
  /// Gets or sets the date and time when the event happened.
  /// </summary>
  public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

  /// <summary>
  /// Gets or sets the delete action performed by the event.
  /// </summary>
  public DeleteAction DeleteAction { get; set; }
}
