namespace Logitar.EventSourcing;

public interface IEventStore
{
  StreamId Append(StreamId? streamId, Type? streamType, StreamExpectation streamExpectation, IEnumerable<IEvent> events);

  bool HasChanges { get; }
  void ClearChanges();

  Task<Stream?> FetchAsync(StreamId streamId, FetchOptions? options = null, CancellationToken cancellationToken = default);

  Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
