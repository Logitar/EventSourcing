namespace Logitar.EventSourcing;

/// <summary>
/// Represents a domain event that has been raised by an <see cref="AggregateRoot"/> and can be applied to it.
/// </summary>
public abstract class DomainEvent
{
  /// <summary>
  /// Gets or sets the identifier of the event.
  /// </summary>
  public EventId Id { get; set; } = EventId.NewId();

  /// <summary>
  /// Gets or sets the identifier of the aggregate to which the event belongs to.
  /// </summary>
  public AggregateId AggregateId { get; set; }
  /// <summary>
  /// Gets or sets the version of the event.
  /// </summary>
  public long Version { get; set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who triggered the event.
  /// </summary>
  public ActorId ActorId { get; set; }
  /// <summary>
  /// Gets or sets the date and time when the event occurred.
  /// </summary>
  public DateTime OccurredOn { get; set; } = DateTime.Now;

  /// <summary>
  /// Gets or sets a value indicating whether or not the aggregate is deleted.
  /// </summary>
  public bool? IsDeleted { get; set; }

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the domain event.
  /// </summary>
  /// <param name="obj">The object to be compared to.</param>
  /// <returns>True if the object is equal to the domain event.</returns>
  public override bool Equals(object obj) => obj is DomainEvent @event && @event.Id == Id;
  /// <summary>
  /// Returns the hash code of the current domain event.
  /// </summary>
  /// <returns>The hash code.</returns>
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  /// <summary>
  /// Returns a string representation of the domain event.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => $"{GetType()} (Id={Id})";
}
