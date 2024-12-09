namespace Logitar.EventSourcing;

public interface IEventStore
{
  StreamId Append(StreamId? streamId, Type? streamType, StreamExpectation streamExpectation, IEnumerable<IEvent> events);

  bool HasChanges { get; }
  void ClearChanges();

  Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
