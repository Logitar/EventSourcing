namespace Logitar.EventSourcing;

public interface IAggregate
{
  AggregateId Id { get; }

  bool HasChanges { get; }
  IReadOnlyCollection<IEvent> Changes { get; }
  void ClearChanges();
}
