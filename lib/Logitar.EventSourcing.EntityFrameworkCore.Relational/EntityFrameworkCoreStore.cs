using Logitar.EventSourcing.EntityFrameworkCore.Relational.Entities;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

public class EntityFrameworkCoreStore : EventStore
{
  protected EventContext Context { get; }
  protected IEventConverter Converter { get; }

  public EntityFrameworkCoreStore(IEnumerable<IEventBus> buses, EventContext context, IEventConverter converter) : base(buses)
  {
    Context = context;
    Converter = converter;
  }

  public override Task<Stream?> FetchAsync(StreamId streamId, FetchOptions? options, CancellationToken cancellationToken)
  {
    options ??= options ?? new FetchOptions();

    throw new NotImplementedException();
  }

  public override async Task SaveChangesAsync(CancellationToken cancellationToken)
  {
    Queue<IEvent> events = [];

    HashSet<string> streamIds = Changes.Select(stream => stream.Id.Value).ToHashSet();
    Dictionary<string, StreamEntity> streams = await Context.Streams.ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

    while (HasChanges)
    {
      AppendToStream appendedStream = DequeueChange();

      _ = streams.TryGetValue(appendedStream.Id.Value, out StreamEntity? stream);

      EnforceStreamExpectation(appendedStream, stream);

      if (stream == null)
      {
        stream = new StreamEntity(appendedStream.Id, appendedStream.Type);
        streams[stream.Id] = stream;

        Context.Streams.Add(stream);
      }

      foreach (IEvent appendedEvent in appendedStream.Events)
      {
        EventEntity @event = Converter.ToEventEntity(appendedEvent, stream);
        stream.Append(@event);

        events.Enqueue(appendedEvent);
      }
    }

    await Context.SaveChangesAsync(cancellationToken);

    await PublishAsync(events, cancellationToken);
  }

  protected virtual void EnforceStreamExpectation(AppendToStream appendedStream, StreamEntity? stream)
  {
    StreamExpectation expectation = appendedStream.Expectation;

    switch (expectation.Kind)
    {
      case StreamExpectation.StreamExpectationKind.ShouldBeAtVersion:
        long expectedVersion = expectation.Version - appendedStream.Events.Count();
        if ((stream?.Version ?? 0) != expectedVersion)
        {
          throw new NotImplementedException();
        }
        break;
      case StreamExpectation.StreamExpectationKind.ShouldExist:
        if (stream == null)
        {
          throw new NotImplementedException();
        }
        break;
      case StreamExpectation.StreamExpectationKind.ShouldNotExist:
        if (stream != null)
        {
          throw new NotImplementedException();
        }
        break;
    }
  }
}
