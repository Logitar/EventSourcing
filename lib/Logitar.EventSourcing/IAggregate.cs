namespace Logitar.EventSourcing;

/// <summary>
/// Represents a domain aggregate, which should be a tangible concept in a domain model.
/// </summary>
public interface IAggregate
{
  /// <summary>
  /// Gets the identifier of the aggregate.
  /// </summary>
  StreamId Id { get; }

  /// <summary>
  /// Gets a value indicating whether or not the aggregate has uncommitted changes.
  /// </summary>
  bool HasChanges { get; }
  /// <summary>
  /// Gets the uncommitted changes of the aggregate.
  /// </summary>
  IReadOnlyCollection<IEvent> Changes { get; }
  /// <summary>
  /// Clears the uncommitted changes of the aggregate.
  /// </summary>
  void ClearChanges();

  /// <summary>
  /// Loads an aggregate from its changes and assign its identifier.
  /// </summary>
  /// <param name="id">The identifier of the aggregate.</param>
  /// <param name="changes">The changes of the aggregate.</param>
  void LoadFromChanges(StreamId id, IEnumerable<IEvent> changes);
}
