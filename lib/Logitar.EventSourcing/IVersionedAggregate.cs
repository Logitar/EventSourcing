namespace Logitar.EventSourcing;

/// <summary>
/// Represents an aggregate that is versioned.
/// </summary>
public interface IVersionedAggregate : IAggregate
{
  /// <summary>
  /// Gets the version of the aggregate.
  /// </summary>
  long Version { get; }
}
