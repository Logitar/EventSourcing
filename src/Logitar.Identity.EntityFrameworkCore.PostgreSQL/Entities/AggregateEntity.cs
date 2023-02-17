using Logitar.EventSourcing;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model representing an aggregate.
/// </summary>
public abstract class AggregateEntity
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateEntity"/> class.
  /// </summary>
  protected AggregateEntity()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateEntity"/> class to the state of the specified event.
  /// </summary>
  /// <param name="e">The creation event.</param>
  protected AggregateEntity(DomainEvent e)
  {
    AggregateId = e.AggregateId.Value;
    Version = e.Version;

    CreatedById = e.ActorId.Value;
    CreatedOn = e.OccurredOn;
  }

  /// <summary>
  /// Gets the aggregate identifier.
  /// </summary>
  public string AggregateId { get; private set; } = string.Empty;
  /// <summary>
  /// Gets the aggregate version.
  /// </summary>
  public long Version { get; private set; }

  /// <summary>
  /// Gets the identifier of the actor who created the aggregate.
  /// </summary>
  public string CreatedById { get; private set; } = string.Empty;
  /// <summary>
  /// Gets the date and time when the aggregate was created.
  /// </summary>
  public DateTime CreatedOn { get; private set; }

  /// <summary>
  /// Gets the identifier of the actor who updated the aggregate lastly.
  /// </summary>
  public string? UpdatedById { get; private set; }
  /// <summary>
  /// Gets the date and time when the aggregate was updated lastly.
  /// </summary>
  public DateTime? UpdatedOn { get; private set; }

  /// <summary>
  /// Sets the version of the aggregate to the version of the specified event.
  /// </summary>
  /// <param name="e">The domain event.</param>
  protected void SetVersion(DomainEvent e)
  {
    Version = e.Version;
  }
  /// <summary>
  /// Updates the aggregate to the state of the specified event.
  /// </summary>
  /// <param name="e">The update event.</param>
  protected void Update(DomainEvent e)
  {
    SetVersion(e);

    UpdatedById = e.ActorId.Value;
    UpdatedOn = e.OccurredOn;
  }
}
