using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Logitar.EventSourcing;

public interface IEventStore
{
  #region TODO(fpion): extension methods
  void Append(StreamId streamId, params object[] events);
  void Append(StreamId streamId, IEnumerable<object> events);
  void Append(StreamId streamId, StreamExpectation expectation, params object[] events);
  void Append(StreamId streamId, StreamExpectation expectation, IEnumerable<object> events);

  // TODO(fpion): AppendAsync methods, identical, call SaveChangesAsync directly after Append
  #endregion

  /* TODO(fpion): KURRENT
   * ⚠️ Version/Revision starts at 0!
   * StreamState:
   * - NoStream: stream should not exist before append (version < 0)
   * - Any: don't care
   * - StreamExists: stream should exist before append (version >= 0)
   * StreamRevision:
   * - None: stream should not exist before append (version < 0)
   * - FromInt64: stream should be at specified version before append (version >= 0)
   */

  /* TODO(fpion): MARTEN
   * ⚠️ Version/Revision starts at 1.
   * expectedVersion: stream should be at specified version after append (version > 0)
   * expectedVersion is used to calculate current stream version before append, so anticipatedVersion = expectedVersion - events.Count
   */

  Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
