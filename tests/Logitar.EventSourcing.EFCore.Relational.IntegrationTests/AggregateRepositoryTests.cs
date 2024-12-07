using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

public abstract class AggregateRepositoryTests : Infrastructure.AggregateRepositoryTests
{
  private EventContext? _context = null;

  protected abstract EventContext CreateEventContext();

  protected override void AssertEqual(IEventEntity _expected, IEventEntity _actual)
  {
    if (_expected is not EventEntity expected)
    {
      throw new ArgumentException($"The entity must be an instance of the {typeof(EventEntity)} class.", nameof(_expected));
    }
    if (_actual is not EventEntity actual)
    {
      throw new ArgumentException($"The entity must be an instance of the {typeof(EventEntity)} class.", nameof(_actual));
    }

    Assert.Equal(expected.Id, actual.Id);
    Assert.Equal(expected.ActorId, actual.ActorId);
    Assert.Equal(expected.IsDeleted, actual.IsDeleted);
    Assert.Equal(ToUnixTimeMilliseconds(expected.OccurredOn), ToUnixTimeMilliseconds(actual.OccurredOn));
    Assert.Equal(expected.Version, actual.Version);
    Assert.Equal(expected.AggregateType, actual.AggregateType);
    Assert.Equal(expected.AggregateId, actual.AggregateId);
    Assert.Equal(expected.EventType, actual.EventType);
    Assert.Equal(expected.EventData, actual.EventData);
  }
  private static long ToUnixTimeMilliseconds(DateTime moment)
  {
    // NOTE(fpion): DateTimes stored to SQL Server are retrieved as Unspecified, even if they are stored as UTC.
    if (moment.Kind == DateTimeKind.Unspecified)
    {
      moment = new DateTime(moment.Ticks, DateTimeKind.Utc);
    }

    return new DateTimeOffset(moment).ToUnixTimeMilliseconds();
  }

  protected override Infrastructure.AggregateRepository CreateRepository(IEventBus eventBus)
  {
    _context = CreateEventContext();

    _context.Database.EnsureDeleted();
    _context.Database.Migrate();

    return new AggregateRepository(eventBus, _context, EventSerializer);
  }

  protected override IEnumerable<IEventEntity> GetEventEntities(AggregateRoot aggregate)
  {
    return EventEntity.FromChanges(aggregate, EventSerializer);
  }

  protected override async Task<IEnumerable<IEventEntity>> LoadEventsAsync(CancellationToken cancellationToken)
  {
    Assert.NotNull(_context);

    return await _context.Events.AsNoTracking().ToArrayAsync(cancellationToken);
  }

  protected override async Task SeedDatabaseAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken)
  {
    Assert.NotNull(_context);

    IEnumerable<EventEntity> events = aggregates.SelectMany(aggregate => EventEntity.FromChanges(aggregate, EventSerializer));
    _context.Events.AddRange(events);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
