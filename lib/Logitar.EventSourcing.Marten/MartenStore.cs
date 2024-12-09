using Logitar.EventSourcing.Infrastructure;
using Marten;

namespace Logitar.EventSourcing.Marten_; // TODO(fpion): issue

public class MartenStore : EventStore
{
  protected IDocumentStore DocumentStore { get; }

  public MartenStore(IEnumerable<IEventBus> buses, IDocumentStore documentStore) : base(buses)
  {
    DocumentStore = documentStore;
  }

  public override async Task<Stream?> FetchAsync(StreamId streamId, FetchOptions? options, CancellationToken cancellationToken)
  {
    options ??= new FetchOptions();

    using IDocumentSession session = await DocumentStore.LightweightSerializableSessionAsync(cancellationToken);

    Guid? streamGuid = null;
    try
    {
      streamGuid = streamId.ToGuid();
    }
    catch (Exception)
    {
    }

    long version = 0; // TODO(fpion): implement
    DateTimeOffset? timestamp = null; // TODO(fpion): implement
    long fromVersion = 0; // TODO(fpion): implement

    IReadOnlyList<Marten.Events.IEvent> events;
    if (streamGuid.HasValue)
    {
      events = await session.Events.FetchStreamAsync(streamGuid.Value, version, timestamp, fromVersion, cancellationToken);
    }
    else
    {
      events = await session.Events.FetchStreamAsync(streamId.Value, version, timestamp, fromVersion, cancellationToken);
    }

    Type? type = null; // TODO(fpion): implement
    Event[]? e = null; // TODO(fpion): implement

    return new Stream(streamId, type, e);

    // Id: Guid
    // Version: long
    // Sequence: long
    // Data: object
    // StreamId: Guid
    // StreamKey?: string
    // Timestamp: DateTimeOffset
    // TenantId: string
    // EventType: Type
    // EventTypeName: string
    // DotNetTypeName: string
    // CausationId: string?
    // CorrelationId: string?
    // Headers: Dictionary<string, object>?
    // IsArchived: bool
    // AggregateTypeName: string? (unavailable)
  }

  public override async Task SaveChangesAsync(CancellationToken cancellationToken) // TODO(fpion): handle StreamExpectationKind.ShouldExist
  {
    using IDocumentSession session = await DocumentStore.LightweightSerializableSessionAsync(cancellationToken);

    Queue<IEvent> events = [];

    while (HasChanges)
    {
      AppendToStream stream = DequeueChange();
      Guid? streamId = null;
      try
      {
        streamId = stream.Id.ToGuid();
      }
      catch (Exception)
      {
      }

      if (IsNewStream(stream))
      {
        if (streamId.HasValue)
        {
          session.Events.StartStream(stream.Type, streamId.Value, stream.Events);
        }
        else
        {
          session.Events.StartStream(stream.Type, stream.Id.Value, stream.Events);
        }
      }
      else
      {
        if (streamId.HasValue)
        {
          if (stream.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion)
          {
            session.Events.Append(streamId.Value, stream.Expectation.Version, stream.Events);
          }
          else
          {
            session.Events.Append(streamId.Value, stream.Events);
          }
        }
        else
        {
          if (stream.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion)
          {
            session.Events.Append(stream.Id.Value, stream.Expectation.Version, stream.Events);
          }
          else
          {
            session.Events.Append(stream.Id.Value, stream.Events);
          }
        }
      }

      foreach (IEvent @event in stream.Events)
      {
        events.Enqueue(@event);
      }
    }

    await session.SaveChangesAsync(cancellationToken);

    await PublishAsync(events, cancellationToken);
  }

  protected virtual bool IsNewStream(AppendToStream stream) => stream.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldNotExist
    || (stream.Expectation.Kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion && stream.Expectation.Version == stream.Events.Count());
}
