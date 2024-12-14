namespace Logitar.EventSourcing;

/// <summary>
/// Represents the state of an aggregate at a precise point in time.
/// </summary>
public class AggregateSnapshot
{
  /// <summary>
  /// Gets or sets the version of the aggregate.
  /// </summary>
  public long Version { get; set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who created the aggregate.
  /// </summary>
  public ActorId? CreatedBy { get; set; }
  /// <summary>
  /// Gets or sets the date and time when the aggregate was created.
  /// </summary>
  public DateTime CreatedOn { get; set; }

  /// <summary>
  /// Gets or sets the identifier of the actor who updated the aggregate lastly.
  /// </summary>
  public ActorId? UpdatedBy { get; set; }
  /// <summary>
  /// Gets or sets the date and time when the aggregate was updated lastly.
  /// </summary>
  public DateTime UpdatedOn { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether or not the aggregate is deleted.
  /// </summary>
  public bool IsDeleted { get; set; }
}
