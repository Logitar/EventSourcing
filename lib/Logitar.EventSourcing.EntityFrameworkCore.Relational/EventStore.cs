﻿using Logitar.EventSourcing.Infrastructure;
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
  /// Fetches an event stream from the store.
  /// </summary>
  /// <param name="streamId">The identifier of the stream.</param>
  /// <param name="options">The fetch options.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The retrieved stream, or null if it was not found.</returns>
  public override Task<Stream?> FetchAsync(StreamId streamId, FetchOptions? options, CancellationToken cancellationToken)
  {
    options ??= new FetchOptions();

    Stream? stream = null; // TODO(fpion): implement

    return Task.FromResult(stream);
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

        EventEntity entity = Converter.ToEventEntity(@event);
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
          throw new NotImplementedException(); // TODO(fpion): implement and document
        }
        break;
      case StreamExpectation.StreamExpectationKind.ShouldExist:
        if (stream == null)
        {
          throw new NotImplementedException(); // TODO(fpion): implement and document
        }
        break;
      case StreamExpectation.StreamExpectationKind.ShouldNotExist:
        if (stream != null)
        {
          throw new NotImplementedException(); // TODO(fpion): implement and document
        }
        break;
    }
  }
}