namespace Logitar.EventSourcing;

/// <summary>
/// Represents an aggregate that can be deleted.
/// </summary>
public interface IDeletableAggregate : IAggregate
{
  /// <summary>
  /// Gets a value indicating whether or not the aggregate is deleted.
  /// </summary>
  bool IsDeleted { get; }
}
