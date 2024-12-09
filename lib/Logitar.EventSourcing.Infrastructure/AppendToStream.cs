namespace Logitar.EventSourcing.Infrastructure;

public record AppendToStream(StreamId Id, Type? Type, StreamExpectation Expectation, IEnumerable<IEvent> Events);
