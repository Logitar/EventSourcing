using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// Represents an event store in which events can be appended or retrieved from a Relational Entity Framework Core database.
/// </summary>
public class EventStore : Infrastructure.EventStore
{
  /// <summary>
  /// Gets the database context.
  /// </summary>
  protected EventContext Context { get; }
  /// <summary>
  /// Gets the event converter.
  /// </summary>
  protected IEventConverter Converter { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventStore"/> class.
  /// </summary>
  /// <param name="buses">The event buses.</param>
  /// <param name="context">The database context.</param>
  /// <param name="converter">The event converter.</param>
  public EventStore(IEnumerable<IEventBus> buses, EventContext context, IEventConverter converter) : base(buses)
  {
    Context = context;
    Converter = converter;
  }

  /// <summary>
  /// Fetches many event streams from the store.
  /// </summary>
  /// <param name="options">The fetch options.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved streams, or an empty collection if none was found.</returns>
  public override async Task<IReadOnlyCollection<Stream>> FetchAsync(FetchManyOptions? options, CancellationToken cancellationToken)
  {
    IQueryable<EventEntity> query = Context.Events.AsNoTracking().Include(x => x.Stream);

    options ??= new FetchManyOptions();

    if (options.StreamTypes.Count > 0)
    {
      bool isNullTypeAllowed = false;
      HashSet<string> streamTypes = new(capacity: options.StreamTypes.Count);
      foreach (Type? type in options.StreamTypes)
      {
        if (type == null)
        {
          isNullTypeAllowed = true;
        }
        else
        {
          streamTypes.Add(type.GetNamespaceQualifiedName());
        }
      }

      query = isNullTypeAllowed
        ? query.Where(x => x.Stream!.Type == null || streamTypes.Contains(x.Stream.Type))
        : query.Where(x => x.Stream!.Type != null && streamTypes.Contains(x.Stream.Type));
    }
    if (options.StreamIds.Count > 0)
    {
      HashSet<string> streamIds = options.StreamIds.Select(id => id.Value).ToHashSet();
      query = query.Where(x => streamIds.Contains(x.Stream!.Id));
    }

    if (options.FromVersion > 0)
    {
      query = query.Where(x => x.Version >= options.FromVersion);
    }
    if (options.ToVersion > 0)
    {
      query = query.Where(x => x.Version <= options.ToVersion);
    }
    if (options.Actor != null)
    {
      string? actorId = options.Actor.ActorId?.Value;
      query = query.Where(x => x.ActorId == actorId);
    }
    if (options.OccurredFrom.HasValue)
    {
      DateTime occurredFrom = options.OccurredFrom.Value.AsUniversalTime();
      query = query.Where(x => x.OccurredOn >= occurredFrom);
    }
    if (options.OccurredTo.HasValue)
    {
      DateTime occurredTo = options.OccurredTo.Value.AsUniversalTime();
      query = query.Where(x => x.OccurredOn <= occurredTo);
    }
    if (options.IsDeleted.HasValue)
    {
      query = query.Where(x => x.Stream!.IsDeleted == options.IsDeleted.Value);
    }

    EventEntity[] entities = await query.OrderBy(x => x.Version).ToArrayAsync(cancellationToken);

    Dictionary<StreamId, List<Event>> events = [];
    Dictionary<StreamId, HashSet<Type>> types = [];
    foreach (EventEntity entity in entities)
    {
      if (entity.Stream == null)
      {
        throw new InvalidOperationException("The event stream entity is required.");
      }

      StreamId streamId = new(entity.Stream.Id);
      Event @event = Converter.ToEvent(entity);

      if (!events.TryGetValue(streamId, out List<Event>? streamEvents))
      {
        streamEvents = [];
        events[streamId] = streamEvents;
      }
      streamEvents.Add(@event);

      Type? type = entity.Stream.GetStreamType();
      if (type != null)
      {
        if (!types.TryGetValue(streamId, out HashSet<Type>? streamTypes))
        {
          streamTypes = [];
          types[streamId] = streamTypes;
        }
        streamTypes.Add(type);
      }
    }

    List<Stream> streams = new(capacity: events.Count);
    foreach (KeyValuePair<StreamId, List<Event>> streamEvents in events)
    {
      Type? type = null;
      if (types.TryGetValue(streamEvents.Key, out HashSet<Type>? streamTypes) && streamTypes.Count == 1)
      {
        type = streamTypes.Single();
      }

      Stream stream = new(streamEvents.Key, type, streamEvents.Value);
      streams.Add(stream);
    }

    return streams.AsReadOnly();
  }
  /// <summary>
  /// Fetches an event stream from the store.
  /// </summary>
  /// <param name="streamId">The identifier of the stream.</param>
  /// <param name="options">The fetch options.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved stream, or null if it was not found.</returns>
  public override async Task<Stream?> FetchAsync(StreamId streamId, FetchOptions? options, CancellationToken cancellationToken)
  {
    IQueryable<EventEntity> query = Context.Events.AsNoTracking()
      .Include(x => x.Stream)
      .Where(x => x.Stream!.Id == streamId.Value);

    options ??= new FetchOptions();
    if (options.FromVersion > 0)
    {
      query = query.Where(x => x.Version >= options.FromVersion);
    }
    if (options.ToVersion > 0)
    {
      query = query.Where(x => x.Version <= options.ToVersion);
    }
    if (options.Actor != null)
    {
      string? actorId = options.Actor.ActorId?.Value;
      query = query.Where(x => x.ActorId == actorId);
    }
    if (options.OccurredFrom.HasValue)
    {
      DateTime occurredFrom = options.OccurredFrom.Value.AsUniversalTime();
      query = query.Where(x => x.OccurredOn >= occurredFrom);
    }
    if (options.OccurredTo.HasValue)
    {
      DateTime occurredTo = options.OccurredTo.Value.AsUniversalTime();
      query = query.Where(x => x.OccurredOn <= occurredTo);
    }
    if (options.IsDeleted.HasValue)
    {
      query = query.Where(x => x.Stream!.IsDeleted == options.IsDeleted.Value);
    }

    EventEntity[] entities = await query.OrderBy(x => x.Version).ToArrayAsync(cancellationToken);
    if (entities.Length <= 0)
    {
      return null;
    }
    StreamEntity stream = entities.First().Stream ?? throw new InvalidOperationException("The event stream entity is required.");

    Type? type = stream.GetStreamType();
    IEnumerable<Event> events = entities.Select(Converter.ToEvent);

    return new Stream(streamId, type, events);
  }

  /// <summary>
  /// Saves the unsaved changes in the event store.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public override async Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    Queue<IEvent> events = [];

    HashSet<string> streamIds = Changes.Select(stream => stream.Id.Value).ToHashSet();
    Dictionary<string, StreamEntity> streams = await Context.Streams
      .Where(x => streamIds.Contains(x.Id))
      .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

    while (HasChanges)
    {
      AppendToStream appendToStream = DequeueChange();

      _ = streams.TryGetValue(appendToStream.Id.Value, out StreamEntity? stream);

      EnforceStreamExpectation(appendToStream, stream);

      if (stream == null)
      {
        stream = new StreamEntity(appendToStream.Id, appendToStream.Type);
        streams[stream.Id] = stream;

        Context.Streams.Add(stream);
      }

      foreach (IEvent @event in appendToStream.Events)
      {
        events.Enqueue(@event);

        EventEntity entity = Converter.ToEventEntity(@event, stream);
        stream.Append(entity);
      }
    }

    await Context.SaveChangesAsync(cancellationToken);

    await PublishAsync(events, cancellationToken);
  }

  /// <summary>
  /// Enforces the specified stream expectation unto the specified stream.
  /// </summary>
  /// <param name="appendToStream">The operation containing the stream expectation.</param>
  /// <param name="stream">The stream to enforce the expectation unto.</param>
  protected virtual void EnforceStreamExpectation(AppendToStream appendToStream, StreamEntity? stream)
  {
    switch (appendToStream.Expectation.Kind)
    {
      case StreamExpectation.StreamExpectationKind.ShouldBeAtVersion:
        long anticipatedVersion = appendToStream.Expectation.Version - appendToStream.Events.Count();
        if ((stream?.Version ?? 0) != anticipatedVersion)
        {
          throw new InvalidOperationException($"The stream 'Id={appendToStream.Id}' was expected to be at version {anticipatedVersion}, but was found at version {stream?.Version ?? 0}.");
        }
        break;
      case StreamExpectation.StreamExpectationKind.ShouldExist:
        if (stream == null)
        {
          throw new InvalidOperationException($"The stream 'Id={appendToStream.Id}' was expected to exist, but does not exist.");
        }
        break;
      case StreamExpectation.StreamExpectationKind.ShouldNotExist:
        if (stream != null)
        {
          throw new InvalidOperationException($"The stream 'Id={appendToStream.Id}' was not expected to exist, but was found at version {stream.Version}.");
        }
        break;
    }
  }
}
