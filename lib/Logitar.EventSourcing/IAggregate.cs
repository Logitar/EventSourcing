namespace Logitar.EventSourcing;

public interface IAggregate
{
  StreamId Id { get; }

  bool HasChanges { get; }
  IReadOnlyCollection<IEvent> Changes { get; }
  void ClearChanges();
}
