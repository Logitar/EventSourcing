using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

[Trait(Traits.Category, Categories.Integration)]
public class EventStoreTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly EventContext _context;
  private readonly Mock<IEventBus> _eventBus = new();

  private readonly EventStore _store;

  public EventStoreTests()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddUserSecrets("33f5c7d2-871a-4d4c-a458-86cad5efddb1")
      .Build();

    DbContextOptions<EventContext> options = new DbContextOptionsBuilder<EventContext>()
      .UseNpgsql(configuration.GetConnectionString(nameof(EventContext)))
      .Options;
    _context = new EventContext(options);

    _context.Database.EnsureDeleted();
    _context.Database.EnsureCreated();

    _store = new(_context, _eventBus.Object);
  }

  [Fact]
  public async Task Given_aggregate_has_changes_When_saved_Then_events_are_saved_and_published()
  {
    TestAggregate aggregate = new();
    aggregate.Rename("Test");
    aggregate.Delete();
    Assert.True(aggregate.HasChanges);
    IReadOnlyCollection<DomainEvent> changes = aggregate.Changes;
    Assert.Empty(_context.Events);

    await _store.SaveAsync(aggregate, _cancellationToken);

    HashSet<Guid> savedEvents = _context.Events.Select(x => x.Id).ToHashSet();
    foreach (DomainEvent change in changes)
    {
      Assert.Contains(change.Id, savedEvents);
      _eventBus.Verify(x => x.PublishAsync(change, _cancellationToken), Times.Once);
    }

    Assert.False(aggregate.HasChanges);
  }

  [Fact]
  public async Task Given_aggregate_has_no_changes_When_saved_Then_no_event_saved_nor_published()
  {
    TestAggregate aggregate = new();
    aggregate.Delete();
    aggregate.ClearChanges();
    Assert.False(aggregate.HasChanges);
    Assert.Empty(_context.Events);

    await _store.SaveAsync(aggregate, _cancellationToken);

    Assert.Empty(_context.Events);
    _eventBus.Verify(x => x.PublishAsync(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Theory]
  [InlineData("6F8900BA-B566-449E-AA61-52EED1A65A32")]
  public async Task Given_aggregate_When_loaded_by_ID_and_version_Then_aggregate_correct_version(string aggregateId)
  {
    AggregateId id = new(Guid.Parse(aggregateId));
    TestAggregate aggregate = new(id);
    aggregate.Rename("Test");
    aggregate.Rename("Charles");
    aggregate.Delete();

    IEnumerable<EventEntity> events = EventEntity.FromChanges(aggregate);
    _context.Events.AddRange(events);
    await _context.SaveChangesAsync();

    TestAggregate result = (await _store.LoadAsync<TestAggregate>(id, version: 1, includeDeleted: false, _cancellationToken))!;
    Assert.NotNull(result);
    Assert.Equal("Test", result.Name);
  }

  [Theory]
  [InlineData("C82724C8-9249-4382-9781-D709DEE1F4E8")]
  public async Task Given_aggregate_When_loaded_by_ID_Then_aggregate(string aggregateId)
  {
    AggregateId id = new(Guid.Parse(aggregateId));
    TestAggregate aggregate = new(id);
    aggregate.Rename("Test");

    IEnumerable<EventEntity> events = EventEntity.FromChanges(aggregate);
    _context.Events.AddRange(events);
    await _context.SaveChangesAsync();

    TestAggregate result = (await _store.LoadAsync<TestAggregate>(id, version: null, includeDeleted: false, _cancellationToken))!;
    Assert.Equal(aggregate, result);
  }

  [Theory]
  [InlineData("C82724C8-9249-4382-9781-D709DEE1F4E8", "327B137F-BF97-46C1-AE02-4A487CCF0F32")]
  public async Task Given_aggregates_When_loaded_by_IDs_Then_aggregates(params string[] aggregateIds)
  {
    IEnumerable<AggregateId> ids = aggregateIds.Select(id => new AggregateId(Guid.Parse(id)));
    Dictionary<AggregateId, TestAggregate> aggregates = ids.Select(id =>
    {
      TestAggregate aggregate = new(id);
      aggregate.Rename("Test");
      return aggregate;
    }).ToDictionary(x => x.Id, x => x);

    IEnumerable<EventEntity> events = aggregates.Values.SelectMany(EventEntity.FromChanges);
    _context.Events.AddRange(events);
    await _context.SaveChangesAsync();

    IEnumerable<TestAggregate> results = await _store.LoadAsync<TestAggregate>(ids, includeDeleted: false, _cancellationToken);
    Assert.Equal(aggregates.Count, results.Count());
    foreach (TestAggregate result in results)
    {
      TestAggregate aggregate = aggregates[result.Id];
      Assert.Equal(aggregate, result);
    }
  }

  [Theory]
  [InlineData("317E1494-72B8-4218-9193-BFC40F8FA5E7", "D9F83740-F916-4D9A-9EA2-CF11B434981B")]
  public async Task Given_aggregates_with_changes_When_saved_Then_events_are_saved_and_published(params string[] aggregateIds)
  {
    IEnumerable<AggregateId> ids = aggregateIds.Select(id => new AggregateId(Guid.Parse(id)));
    TestAggregate[] aggregates = ids.Select(id =>
    {
      TestAggregate aggregate = new(id);
      aggregate.Rename("Test");
      return aggregate;
    }).ToArray();
    Assert.True(aggregates.All(a => a.HasChanges));
    DomainEvent[] changes = aggregates.SelectMany(x => x.Changes).ToArray();
    TestAggregate unchanged = new();
    Assert.False(unchanged.HasChanges);
    Assert.Empty(_context.Events);

    await _store.SaveAsync(aggregates.Concat(new[] { unchanged }), _cancellationToken);

    Dictionary<AggregateId, int> counts = _context.Events.GroupBy(x => x.AggregateId).ToDictionary(x => new AggregateId(x.Key), x => x.Count());
    Assert.Equal(aggregates.Length, counts.Count);
    foreach (TestAggregate aggregate in aggregates)
    {
      Assert.Equal(1, counts[aggregate.Id]);
    }

    foreach (DomainEvent change in changes)
    {
      _eventBus.Verify(x => x.PublishAsync(change, _cancellationToken), Times.Once);
    }

    _eventBus.Verify(x => x.PublishAsync(It.Is<DomainEvent>(e => e.AggregateId == unchanged.Id), It.IsAny<CancellationToken>()), Times.Never);

    Assert.True(aggregates.All(a => !a.HasChanges));
  }

  [Theory]
  [InlineData("53857448-1C5D-49F0-B233-C3A89E338110", "305D30EE-9DFB-4785-870E-B25AF5A83608")]
  public async Task Given_aggregates_without_changes_When_saved_Then_no_event_saved_nor_published(params string[] aggregateIds)
  {
    IEnumerable<AggregateId> ids = aggregateIds.Select(id => new AggregateId(Guid.Parse(id)));
    TestAggregate[] aggregates = ids.Select(id => new TestAggregate(id)).ToArray();
    Assert.Empty(_context.Events);

    await _store.SaveAsync(aggregates, _cancellationToken);

    Assert.Empty(_context.Events);

    _eventBus.Verify(x => x.PublishAsync(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Theory]
  [InlineData("0F7B4732-BA00-4081-B81B-B8DE4753E708")]
  public async Task Given_deleted_aggregate_When_loaded_by_ID_including_deleted_Then_aggregate(string aggregateId)
  {
    AggregateId id = new(Guid.Parse(aggregateId));
    TestAggregate aggregate = new(id);
    aggregate.Delete();

    IEnumerable<EventEntity> events = EventEntity.FromChanges(aggregate);
    _context.Events.AddRange(events);
    await _context.SaveChangesAsync();

    TestAggregate result = (await _store.LoadAsync<TestAggregate>(id, version: null, includeDeleted: true, _cancellationToken))!;
    Assert.NotNull(result);
    Assert.Equal(aggregate, result);
  }

  [Theory]
  [InlineData("0F7B4732-BA00-4081-B81B-B8DE4753E708")]
  public async Task Given_deleted_aggregate_When_loaded_by_ID_Then_null(string aggregateId)
  {
    AggregateId id = new(Guid.Parse(aggregateId));
    TestAggregate aggregate = new(id);
    aggregate.Delete();

    IEnumerable<EventEntity> events = EventEntity.FromChanges(aggregate);
    _context.Events.AddRange(events);
    await _context.SaveChangesAsync();

    Assert.Null(await _store.LoadAsync<TestAggregate>(id, version: null, includeDeleted: false, _cancellationToken));
  }

  [Theory]
  [InlineData("3E3FD551-66DA-4ACA-9E8E-922389A66032", "BBDAB813-59C8-4F45-84E4-6AD93B68562C")]
  public async Task Given_deleted_aggregates_When_loaded_by_IDs_including_deleted_Then_aggregates(params string[] aggregateIds)
  {
    IEnumerable<AggregateId> ids = aggregateIds.Select(id => new AggregateId(Guid.Parse(id)));
    Dictionary<AggregateId, TestAggregate> aggregates = ids.Select(id =>
    {
      TestAggregate aggregate = new(id);
      aggregate.Delete();
      return aggregate;
    }).ToDictionary(x => x.Id, x => x);

    IEnumerable<EventEntity> events = aggregates.Values.SelectMany(EventEntity.FromChanges);
    _context.Events.AddRange(events);
    await _context.SaveChangesAsync();

    IEnumerable<TestAggregate> results = await _store.LoadAsync<TestAggregate>(ids, includeDeleted: true, _cancellationToken);
    Assert.Equal(aggregates.Count, results.Count());
    foreach (TestAggregate result in results)
    {
      TestAggregate aggregate = aggregates[result.Id];
      Assert.Equal(aggregate, result);
    }
  }

  [Theory]
  [InlineData("2AF2A4CA-F537-4787-84C9-24FA5414CA68", "34B09F3E-0F47-420A-8174-80CECCD5F3CE")]
  public async Task Given_deleted_aggregates_When_loaded_by_IDs_Then_empty(params string[] aggregateIds)
  {
    IEnumerable<AggregateId> ids = aggregateIds.Select(id => new AggregateId(Guid.Parse(id)));
    Dictionary<AggregateId, TestAggregate> aggregates = ids.Select(id =>
    {
      TestAggregate aggregate = new(id);
      aggregate.Delete();
      return aggregate;
    }).ToDictionary(x => x.Id, x => x);

    IEnumerable<EventEntity> events = aggregates.Values.SelectMany(EventEntity.FromChanges);
    _context.Events.AddRange(events);
    await _context.SaveChangesAsync();

    Assert.Empty(await _store.LoadAsync<TestAggregate>(ids, includeDeleted: false, _cancellationToken));
  }

  [Theory]
  [InlineData("F6675EA5-90A4-4666-B515-4070A1BC88F8")]
  public async Task Given_no_aggregate_to_load_When_loaded_by_ID_Then_null(string aggregateId)
  {
    TestAggregate aggregate = new();
    aggregate.Rename("Test");

    IEnumerable<EventEntity> events = EventEntity.FromChanges(aggregate);
    _context.Events.AddRange(events);
    await _context.SaveChangesAsync();

    AggregateId id = new(Guid.Parse(aggregateId));
    Assert.Null(await _store.LoadAsync<TestAggregate>(id, version: null, includeDeleted: true, _cancellationToken));
  }

  [Theory]
  [InlineData("08E24117-1F9C-4839-BADA-36F478016BA7", "81EF880F-35A0-4AAE-AD2F-F335274A2D8D")]
  public async Task Given_no_aggregate_to_load_When_loaded_by_IDs_Then_empty(params string[] aggregateIds)
  {
    TestAggregate aggregate = new();
    aggregate.Rename("Test");

    IEnumerable<EventEntity> events = EventEntity.FromChanges(aggregate);
    _context.Events.AddRange(events);
    await _context.SaveChangesAsync();

    IEnumerable<AggregateId> ids = aggregateIds.Select(id => new AggregateId(Guid.Parse(id)));
    Assert.Empty(await _store.LoadAsync<TestAggregate>(ids, includeDeleted: true, _cancellationToken));
  }
}
