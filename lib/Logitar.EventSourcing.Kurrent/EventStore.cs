using EventStore.Client;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Kurrent;

/// <summary>
/// Implements a store of events using EventStoreDB/Kurrent.
/// </summary>
public sealed class EventStore : IEventStore // TODO(fpion): unit & integration tests
{
  /// <summary>
  /// The uncommitted operations.
  /// </summary>
  private readonly List<AppendToStream> _operations = [];

  /// <summary>
  /// The event buses.
  /// </summary>
  private readonly IEnumerable<IEventBus> _buses;
  /// <summary>
  /// The EventStoreDB/Kurrent client.
  /// </summary>
  private readonly EventStoreClient _client;
  /// <summary>
  /// The event converter.
  /// </summary>
  private readonly IEventConverter _converter;

  /// <summary>
  /// Initializes a new instance of the <see cref="EventStore"/> class.
  /// </summary>
  /// <param name="buses">The event buses.</param>
  /// <param name="client">The EventStoreDB/Kurrent client.</param>
  /// <param name="converter">The event converter.</param>
  public EventStore(IEnumerable<IEventBus> buses, EventStoreClient client, IEventConverter converter)
  {
    _buses = buses;
    _client = client;
    _converter = converter;
  }

  /// <summary>
  /// Appends the specified events to the specified stream.
  /// </summary>
  /// <param name="streamId">The stream identifier (optional).</param>
  /// <param name="type">The stream type (optional).</param>
  /// <param name="expectation">An expectation of the stream state.</param>
  /// <param name="events">The events to append.</param>
  /// <returns>The stream identifier. A new identifier will be randomly generated if none is provided.</returns>
  /// <exception cref="ArgumentException">The stream identifier was null, empty or only white-space.</exception>
  public StreamId Append(StreamId? streamId, Type? type, StreamExpectation expectation, IEnumerable<IEvent> events)
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

    foreach (AppendToStream operation in _operations)
    {
      IEnumerable<EventData> events = operation.Events.Select(@event => _converter.ToEventData(@event, operation.Type));
      if (operation.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion)
      {
        long revision = operation.Expectation.Version - operation.Events.Count() - 1;
        await _client.AppendToStreamAsync(operation.Id.Value, StreamRevision.FromInt64(revision), events, cancellationToken: cancellationToken);
      }
      else
      {
        StreamState state = operation.Expectation.Kind switch
        {
          StreamExpectation.StreamExpectationKind.ShouldExist => StreamState.StreamExists,
          StreamExpectation.StreamExpectationKind.ShouldNotExist => StreamState.NoStream,
          _ => StreamState.Any,
        };
        await _client.AppendToStreamAsync(operation.Id.Value, state, events, cancellationToken: cancellationToken);
      }
    }

    foreach (AppendToStream operation in _operations)
    {
      foreach (IEvent @event in operation.Events)
      {
        foreach (IEventBus bus in _buses)
        {
          await bus.PublishAsync(@event, cancellationToken);
        }
      }
    }

    _operations.Clear(); // TODO(fpion): how to retry?
  }
}
