using Logitar.EventSourcing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Logitar.EventSourcing.Kurrent;

public class EventStore : IEventStore
{
  private readonly List<StreamAppend> _append = [];

  public virtual void Append(StreamId streamId, IEnumerable<object> events)
  {
    Append(streamId, StreamExpectation.None, events);
  }
  public virtual void Append(StreamId streamId, params object[] events)
  {
    Append(streamId, StreamExpectation.None, events);
  }
  public virtual void Append(StreamId streamId, StreamExpectation expectation, IEnumerable<object> events)
  {
    Append(streamId, expectation, events.ToArray());
  }
  public virtual void Append(StreamId streamId, StreamExpectation expectation, params object[] events)
  {
    if (string.IsNullOrWhiteSpace(streamId.Value))
    {
      throw new ArgumentException("The stream identifier cannot be null, empty, nor only white-space.", nameof(streamId));
    }
    if (events.Length < 1)
    {
      return;
    }

    _append.Add(new StreamAppend(streamId, expectation, events));
  }

  public virtual Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask; // TODO(fpion): implement
  }
}
