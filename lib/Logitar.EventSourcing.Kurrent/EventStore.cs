using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Logitar.EventSourcing.Kurrent;

public class EventStore : IEventStore
{
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
    // TODO(fpion): implement
  }

  public virtual Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask; // TODO(fpion): implement
  }
}
