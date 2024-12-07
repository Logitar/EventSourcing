using System.Collections.Generic;

namespace Logitar.EventSourcing.Infrastructure;

public sealed record StreamAppend
{
  public StreamId Id { get; }
  public StreamExpectation Expectation { get; }
  public IEnumerable<object> Events { get; }

  public StreamAppend(StreamId id, StreamExpectation expectation, IEnumerable<object> events)
  {
    Id = id;
    Expectation = expectation;
    Events = events;
  }
}
