using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Implements a store of events using EventStoreDB/Kurrent.
/// </summary>
public sealed class EventStore : IEventStore
{
  /// <summary>
  /// The uncommitted operations.
  /// </summary>
  private readonly List<AppendToStream> _operations = [];

  /// <summary>
  /// Appends the specified events to the specified stream.
  /// </summary>
  /// <param name="streamId">The stream identifier (optional).</param>
  /// <param name="type">The stream type (optional).</param>
  /// <param name="expectation">An expectation of the stream state.</param>
  /// <param name="events">The events to append.</param>
  /// <returns>The stream identifier. A new identifier will be randomly generated if none is provided.</returns>
  /// <exception cref="ArgumentException">The stream identifier was null, empty or only white-space.</exception>
  public StreamId Append(StreamId? streamId, Type? type, StreamExpectation expectation, IEnumerable<object> events)
  {
    if (streamId.HasValue)
    {
      if (string.IsNullOrWhiteSpace(streamId.Value.Value))
      {
        throw new ArgumentException("The stream identifier cannot be null, empty, nor only white-space.", nameof(streamId));
      }
    }
    else
    {
      streamId = StreamId.NewId();
    }

    if (events.Any())
    {
      AppendToStream operation = new(streamId.Value, type, expectation, events);
      _operations.Add(operation);
    }

    return streamId.Value;
  }

  /// <summary>
  /// Save the changes to the event store.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public async Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    if (_operations.Count < 1)
    {
      return;
    }

    await Task.Delay(0, cancellationToken); // TODO(fpion): implement
  }
}
