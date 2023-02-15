namespace Logitar.Identity;

/// <summary>
/// The output representation of an aggregate root.
/// </summary>
public abstract record Aggregate
{
  /// <summary>
  /// Gets or sets the current version of the aggregate.
  /// </summary>
  public long Version { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the aggregate was created.
  /// </summary>
  public DateTime CreatedOn { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the aggregate was updated lastly.
  /// </summary>
  public DateTime UpdatedOn { get; set; }
}
