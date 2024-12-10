namespace Logitar.EventSourcing;

public interface IAggregate
{
  StreamId Id { get; }

  bool HasChanges { get; }
  IReadOnlyCollection<IEvent> Changes { get; }
  void ClearChanges();

  void LoadFromChanges(StreamId id, IEnumerable<IEvent> changes);
}
