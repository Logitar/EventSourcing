﻿using Bogus;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

[Trait(Traits.Category, Categories.Unit)]
public class EventStoreTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IEventBus> _bus = new();

  private readonly EventSerializer _serializer = new();
  private readonly EventContext _context;
  private readonly EventConverter _converter;
  private readonly EventStore _store;

  public EventStoreTests()
  {
    _context = new EventContext(new DbContextOptionsBuilder<EventContext>()
      .UseInMemoryDatabase(GetType().Name)
      .Options);
    _context.Database.EnsureDeleted();
    _context.Database.EnsureCreated();

    _converter = new EventConverter(_serializer);
    _store = new EventStore([_bus.Object], _context, _converter);
  }

  [Theory(DisplayName = "FetchAsync: it should apply the specified actor filter (many streams).")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_ActorFilter_When_FetchManyAsync_Then_FilterApplied(bool isNull)
  {
    UserCreated userCreated = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1
    };

    ActorId actorId = new(userCreated.Id.Value);
    UserSignedIn signedIn = new()
    {
      StreamId = userCreated.StreamId,
      Version = 2,
      ActorId = actorId
    };

    SessionCreated sessionCreated = new(userCreated.StreamId)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      ActorId = actorId
    };

    StreamEntity stream1 = new(userCreated.StreamId, typeof(User));
    stream1.Append(_converter.ToEventEntity(userCreated, stream1));
    stream1.Append(_converter.ToEventEntity(signedIn, stream1));

    StreamEntity stream2 = new(sessionCreated.StreamId, typeof(Session));
    stream2.Append(_converter.ToEventEntity(sessionCreated, stream2));

    _context.Streams.AddRange(stream1, stream2);
    _context.SaveChanges();

    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(new FetchManyOptions
    {
      Actor = new ActorFilter(isNull ? null : actorId)
    }, _cancellationToken);

    if (isNull)
    {
      Stream stream = Assert.Single(streams);
      Assert.Equal(userCreated.StreamId, stream.Id);
      Assert.Equal(1, stream.Version);
    }
    else
    {
      Assert.Equal(2, streams.Count);
      Assert.Contains(streams, s => s.Id == userCreated.StreamId && s.CreatedOn == null && s.Events.Single().Data.Equals(signedIn));
      Assert.Contains(streams, s => s.Id == sessionCreated.StreamId);
    }
  }

  [Theory(DisplayName = "FetchAsync: it should apply the specified actor filter (single stream).")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_ActorFilter_When_FetchSingleAsync_Then_FilterApplied(bool isNull)
  {
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddMonths(-1)
    };
    UserPasswordCreated password = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")))
    {
      StreamId = StreamId.NewId(),
      Version = 2,
      ActorId = new ActorId(created.StreamId.ToGuid()),
      OccurredOn = DateTime.Now.AddDays(-1)
    };
    UserSignedIn signedIn = new()
    {
      StreamId = StreamId.NewId(),
      Version = 3,
      ActorId = new ActorId(created.StreamId.ToGuid()),
      OccurredOn = DateTime.Now.AddHours(-1)
    };
    UserDeleted deleted = new()
    {
      StreamId = StreamId.NewId(),
      Version = 4
    };

    StreamEntity entity = new(created.StreamId, typeof(User));
    entity.Append(_converter.ToEventEntity(created, entity));
    entity.Append(_converter.ToEventEntity(password, entity));
    entity.Append(_converter.ToEventEntity(signedIn, entity));
    entity.Append(_converter.ToEventEntity(deleted, entity));
    _context.Streams.Add(entity);
    _context.SaveChanges();

    Stream? stream = await _store.FetchAsync(created.StreamId, new FetchOptions
    {
      Actor = new ActorFilter(isNull ? null : password.ActorId)
    }, _cancellationToken);

    Assert.NotNull(stream);
    Assert.Equal(created.StreamId, stream.Id);
    Assert.Equal(typeof(User), stream.Type);
    Assert.Null(stream.CreatedBy);
    Assert.True(stream.UpdatedOn.HasValue);
    Assert.Equal(isNull, stream.IsDeleted);

    if (isNull)
    {
      Assert.Equal(4, stream.Version);
      Assert.True(stream.CreatedOn.HasValue);
      Assert.Equal(created.OccurredOn.AsUniversalTime(), stream.CreatedOn.Value, TimeSpan.FromSeconds(1));
      Assert.Null(stream.UpdatedBy);
      Assert.Equal(deleted.OccurredOn.AsUniversalTime(), stream.UpdatedOn.Value, TimeSpan.FromSeconds(1));

      Assert.Equal(2, stream.Events.Count);
      Assert.Contains(stream.Events, e => e.Id == created.Id && e.Version == created.Version && e.ActorId == created.ActorId
        && e.OccurredOn == created.OccurredOn.AsUniversalTime() && e.IsDeleted == created.IsDeleted == e.Data.Equals(created));
      Assert.Contains(stream.Events, e => e.Id == deleted.Id && e.Version == deleted.Version && e.ActorId == deleted.ActorId
        && e.OccurredOn == deleted.OccurredOn.AsUniversalTime() && e.IsDeleted == true == e.Data.Equals(deleted));
    }
    else
    {
      Assert.Equal(3, stream.Version);
      Assert.Null(stream.CreatedOn);
      Assert.Equal(signedIn.ActorId, stream.UpdatedBy);
      Assert.Equal(signedIn.OccurredOn.AsUniversalTime(), stream.UpdatedOn.Value, TimeSpan.FromSeconds(1));

      Assert.Equal(2, stream.Events.Count);
      Assert.Contains(stream.Events, e => e.Id == password.Id && e.Version == password.Version && e.ActorId == password.ActorId
        && e.OccurredOn == password.OccurredOn.AsUniversalTime() && e.IsDeleted == password.IsDeleted == e.Data.Equals(password));
      Assert.Contains(stream.Events, e => e.Id == signedIn.Id && e.Version == signedIn.Version && e.ActorId == signedIn.ActorId
        && e.OccurredOn == signedIn.OccurredOn.AsUniversalTime() && e.IsDeleted == signedIn.IsDeleted == e.Data.Equals(signedIn));
    }
  }

  [Fact(DisplayName = "FetchAsync: it should apply the specified date and time filters (many streams).")]
  public async Task Given_DateTimeFilters_When_FetchManyAsync_Then_FiltersApplied()
  {
    UserCreated created1 = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddYears(-1)
    };
    UserCreated created2 = new(_faker.Internet.UserName())
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddDays(-1)
    };
    UserCreated created3 = new(_faker.Internet.UserName())
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddHours(-1)
    };
    UserSignedIn signedIn = new()
    {
      StreamId = created3.StreamId,
      Version = 2
    };

    StreamEntity stream1 = new(created1.StreamId, typeof(User));
    stream1.Append(_converter.ToEventEntity(created1, stream1));

    StreamEntity stream2 = new(created2.StreamId, typeof(User));
    stream2.Append(_converter.ToEventEntity(created2, stream2));

    StreamEntity stream3 = new(created3.StreamId, typeof(User));
    stream3.Append(_converter.ToEventEntity(created3, stream3));

    _context.Streams.AddRange(stream1, stream2, stream3);
    _context.SaveChanges();

    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(new FetchManyOptions
    {
      OccurredFrom = DateTime.Now.AddMonths(-1),
      OccurredTo = DateTime.Now.AddMinutes(-1)
    }, _cancellationToken);

    Assert.Equal(2, streams.Count);
    Assert.Contains(streams, s => s.Id == created2.StreamId && s.Version == 1);
    Assert.Contains(streams, s => s.Id == created3.StreamId && s.Version == 1);
  }

  [Fact(DisplayName = "FetchAsync: it should apply the specified date and time filters (single stream).")]
  public async Task Given_DateTimeFilters_When_FetchSingleAsync_Then_FiltersApplied()
  {
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddMonths(-1)
    };
    UserPasswordCreated password = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")))
    {
      StreamId = StreamId.NewId(),
      Version = 2,
      ActorId = new ActorId(created.StreamId.ToGuid()),
      OccurredOn = DateTime.Now.AddDays(-1)
    };
    UserSignedIn signedIn = new()
    {
      StreamId = StreamId.NewId(),
      Version = 3,
      ActorId = new ActorId(created.StreamId.ToGuid()),
      OccurredOn = DateTime.Now.AddHours(-1)
    };
    UserDeleted deleted = new()
    {
      StreamId = StreamId.NewId(),
      Version = 4
    };

    StreamEntity entity = new(created.StreamId, typeof(User));
    entity.Append(_converter.ToEventEntity(created, entity));
    entity.Append(_converter.ToEventEntity(password, entity));
    entity.Append(_converter.ToEventEntity(signedIn, entity));
    entity.Append(_converter.ToEventEntity(deleted, entity));
    _context.Streams.Add(entity);
    _context.SaveChanges();

    Stream? stream = await _store.FetchAsync(created.StreamId, new FetchOptions
    {
      OccurredFrom = DateTime.Now.AddDays(-7),
      OccurredTo = DateTime.Now.AddMinutes(-15)
    }, _cancellationToken);

    Assert.NotNull(stream);
    Assert.Equal(created.StreamId, stream.Id);
    Assert.Equal(typeof(User), stream.Type);
    Assert.Equal(3, stream.Version);
    Assert.Null(stream.CreatedBy);
    Assert.Null(stream.CreatedOn);
    Assert.Equal(signedIn.ActorId, stream.UpdatedBy);
    Assert.Equal(signedIn.OccurredOn.AsUniversalTime(), stream.UpdatedOn);
    Assert.False(stream.IsDeleted);

    Assert.Equal(2, stream.Events.Count);
    Assert.Contains(stream.Events, e => e.Id == password.Id && e.Version == password.Version && e.ActorId == password.ActorId
      && e.OccurredOn == password.OccurredOn.AsUniversalTime() && e.IsDeleted == password.IsDeleted == e.Data.Equals(password));
    Assert.Contains(stream.Events, e => e.Id == signedIn.Id && e.Version == signedIn.Version && e.ActorId == signedIn.ActorId
      && e.OccurredOn == signedIn.OccurredOn.AsUniversalTime() && e.IsDeleted == signedIn.IsDeleted == e.Data.Equals(signedIn));
  }

  [Fact(DisplayName = "FetchAsync: it should apply the specified stream identifier filter.")]
  public async Task Given_StreamIdentifierFilter_When_FetchAsync_Then_FilterApplied()
  {
    User user1 = new(_faker.Person.UserName);
    User user2 = new(_faker.Internet.UserName());
    Session session = new(user1);

    StreamEntity stream1 = new(user1.Id);
    foreach (IEvent change in user1.Changes)
    {
      stream1.Append(_converter.ToEventEntity(change, stream1));
    }

    StreamEntity stream2 = new(user2.Id);
    foreach (IEvent change in user2.Changes)
    {
      stream2.Append(_converter.ToEventEntity(change, stream2));
    }

    StreamEntity stream3 = new(session.Id);
    foreach (IEvent change in session.Changes)
    {
      stream3.Append(_converter.ToEventEntity(change, stream3));
    }

    _context.Streams.AddRange(stream1, stream2, stream3);
    _context.SaveChanges();

    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(new FetchManyOptions
    {
      StreamIds = [user1.Id, session.Id, StreamId.NewId(), new StreamId()]
    }, _cancellationToken);

    Assert.Equal(2, streams.Count);
    Assert.Contains(streams, s => s.Id == user1.Id);
    Assert.Contains(streams, s => s.Id == session.Id);
  }

  [Theory(DisplayName = "FetchAsync: it should apply the specified stream type filter.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_StreamTypeFilter_When_FetchAsync_Then_FilterApplied(bool isNullTypeAllowed)
  {
    UserCreated userCreated = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1
    };
    SessionCreated sessionCreated = new(userCreated.StreamId)
    {
      StreamId = StreamId.NewId(),
      Version = 1
    };
    SystemStarted systemStarted = new();

    StreamEntity stream1 = new(userCreated.StreamId, typeof(User));
    stream1.Append(_converter.ToEventEntity(userCreated, stream1));

    StreamEntity stream2 = new(sessionCreated.StreamId, typeof(Session));
    stream2.Append(_converter.ToEventEntity(sessionCreated, stream2));

    StreamId systemId = new(Guid.Empty);
    StreamEntity stream3 = new(systemId, type: null);
    stream3.Append(_converter.ToEventEntity(systemStarted, stream3));

    _context.Streams.AddRange(stream1, stream2, stream3);
    _context.SaveChanges();

    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(new FetchManyOptions
    {
      StreamTypes = [typeof(User), isNullTypeAllowed ? null : typeof(Session)]
    }, _cancellationToken);

    Assert.Equal(2, streams.Count);
    Assert.Contains(streams, s => s.Id == userCreated.StreamId);

    if (isNullTypeAllowed)
    {
      Assert.Contains(streams, s => s.Id == systemId);
    }
    else
    {
      Assert.Contains(streams, s => s.Id == sessionCreated.StreamId);
    }
  }

  [Fact(DisplayName = "FetchAsync: it should apply the specified version filters (many streams).")]
  public async Task Given_VersionFilters_When_FetchManyAsync_Then_FiltersApplied()
  {
    UserCreated created1 = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1
    };
    UserCreated created2 = new(_faker.Internet.UserName())
    {
      StreamId = StreamId.NewId(),
      Version = 1
    };
    UserSignedIn signedIn = new()
    {
      StreamId = created2.StreamId,
      Version = 2
    };
    UserDeleted deleted = new()
    {
      StreamId = created2.StreamId,
      Version = 3
    };

    StreamEntity stream1 = new(created1.StreamId, typeof(User));
    stream1.Append(_converter.ToEventEntity(created1, stream1));

    StreamEntity stream2 = new(created2.StreamId, typeof(User));
    stream2.Append(_converter.ToEventEntity(created2, stream2));
    stream2.Append(_converter.ToEventEntity(signedIn, stream2));
    stream2.Append(_converter.ToEventEntity(deleted, stream2));

    _context.Streams.AddRange(stream1, stream2);
    _context.SaveChanges();

    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(new FetchManyOptions
    {
      FromVersion = 1,
      ToVersion = 2
    }, _cancellationToken);

    Assert.Equal(2, streams.Count);
    Assert.Contains(streams, s => s.Id == created1.StreamId && s.Version == 1);
    Assert.Contains(streams, s => s.Id == created2.StreamId && s.Version == 2);
  }

  [Fact(DisplayName = "FetchAsync: it should apply the specified version filters (single stream).")]
  public async Task Given_VersionFilters_When_FetchSingleAsync_Then_FiltersApplied()
  {
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1
    };
    UserPasswordCreated password = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")))
    {
      StreamId = StreamId.NewId(),
      Version = 2,
      ActorId = new ActorId(created.StreamId.ToGuid())
    };
    UserSignedIn signedIn = new()
    {
      StreamId = StreamId.NewId(),
      Version = 3,
      ActorId = new ActorId(created.StreamId.ToGuid())
    };
    UserDeleted deleted = new()
    {
      StreamId = StreamId.NewId(),
      Version = 4
    };

    StreamEntity entity = new(created.StreamId, typeof(User));
    entity.Append(_converter.ToEventEntity(created, entity));
    entity.Append(_converter.ToEventEntity(password, entity));
    entity.Append(_converter.ToEventEntity(signedIn, entity));
    entity.Append(_converter.ToEventEntity(deleted, entity));
    _context.Streams.Add(entity);
    _context.SaveChanges();

    Stream? stream = await _store.FetchAsync(created.StreamId, new FetchOptions
    {
      FromVersion = 2,
      ToVersion = 3
    }, _cancellationToken);

    Assert.NotNull(stream);
    Assert.Equal(created.StreamId, stream.Id);
    Assert.Equal(typeof(User), stream.Type);
    Assert.Equal(3, stream.Version);
    Assert.Null(stream.CreatedBy);
    Assert.Null(stream.CreatedOn);
    Assert.Equal(signedIn.ActorId, stream.UpdatedBy);
    Assert.Equal(signedIn.OccurredOn.AsUniversalTime(), stream.UpdatedOn);
    Assert.False(stream.IsDeleted);

    Assert.Equal(2, stream.Events.Count);
    Assert.Contains(stream.Events, e => e.Id == password.Id && e.Version == password.Version && e.ActorId == password.ActorId
      && e.OccurredOn == password.OccurredOn.AsUniversalTime() && e.IsDeleted == password.IsDeleted == e.Data.Equals(password));
    Assert.Contains(stream.Events, e => e.Id == signedIn.Id && e.Version == signedIn.Version && e.ActorId == signedIn.ActorId
      && e.OccurredOn == signedIn.OccurredOn.AsUniversalTime() && e.IsDeleted == signedIn.IsDeleted == e.Data.Equals(signedIn));
  }

  [Fact(DisplayName = "FetchAsync: it should return a deleted stream.")]
  public async Task Given_DeletedStreamFound_When_FetchAsync_Then_Returned()
  {
    User user = new(_faker.Person.UserName);
    UserCreated? created = Assert.Single(user.Changes) as UserCreated;
    Assert.NotNull(created);

    user.Delete();
    UserDeleted? deleted = Assert.Single(user.Changes.Skip(1)) as UserDeleted;
    Assert.NotNull(deleted);

    StreamEntity entity = new(user.Id, user.GetType());
    foreach (IEvent change in user.Changes)
    {
      entity.Append(_converter.ToEventEntity(change, entity));
    }
    _context.Streams.Add(entity);
    _context.SaveChanges();

    Stream? stream = await _store.FetchAsync(user.Id, new FetchOptions
    {
      IsDeleted = true
    }, _cancellationToken);

    Assert.NotNull(stream);
    Assert.Equal(user.Id, stream.Id);
    Assert.Equal(user.GetType(), stream.Type);
    Assert.Equal(user.Version, stream.Version);
    Assert.Equal(user.CreatedBy, stream.CreatedBy);
    Assert.Equal(user.CreatedOn.AsUniversalTime(), stream.CreatedOn);
    Assert.Equal(user.UpdatedBy, stream.UpdatedBy);
    Assert.Equal(user.UpdatedOn.AsUniversalTime(), stream.UpdatedOn);
    Assert.Equal(user.IsDeleted, stream.IsDeleted);

    Assert.Equal(2, stream.Events.Count);
    Assert.Contains(stream.Events, e => e.Id == created.Id && e.Version == created.Version && e.ActorId == created.ActorId
      && e.OccurredOn == created.OccurredOn.AsUniversalTime() && e.IsDeleted == created.IsDeleted && e.Data.Equals(created));
    Assert.Contains(stream.Events, e => e.Id == deleted.Id && e.Version == deleted.Version && e.ActorId == deleted.ActorId
      && e.OccurredOn == deleted.OccurredOn.AsUniversalTime() && e.IsDeleted == true && e.Data.Equals(deleted));
  }

  [Fact(DisplayName = "FetchAsync: it should return empty when no stream was found.")]
  public async Task Given_NoStream_When_FetchAsync_Then_Empty()
  {
    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(new FetchManyOptions(), _cancellationToken);
    Assert.Empty(streams);
  }

  [Theory(DisplayName = "FetchAsync: it should return matching streams with a deletion status filter.")]
  [InlineData(null)]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_ManyStreams_When_FetchAsync_Then_StreamsMatchingDeletionStatusReturned(bool? isDeleted)
  {
    User user1 = new(_faker.Person.UserName);

    User user2 = new(_faker.Internet.UserName());
    user2.Delete();

    User user3 = new(_faker.Internet.UserName());
    if (isDeleted == true)
    {
      user3.Delete();
    }

    StreamEntity stream1 = new(user1.Id, typeof(User));
    foreach (IEvent change in user1.Changes)
    {
      stream1.Append(_converter.ToEventEntity(change, stream1));
    }

    StreamEntity stream2 = new(user2.Id, typeof(User));
    foreach (IEvent change in user2.Changes)
    {
      stream2.Append(_converter.ToEventEntity(change, stream2));
    }

    StreamEntity stream3 = new(user3.Id, typeof(User));
    foreach (IEvent change in user3.Changes)
    {
      stream3.Append(_converter.ToEventEntity(change, stream3));
    }

    _context.Streams.AddRange(stream1, stream2, stream3);
    _context.SaveChanges();

    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(new FetchManyOptions
    {
      IsDeleted = isDeleted
    }, _cancellationToken);

    switch (isDeleted)
    {
      case false:
        Assert.Equal(2, streams.Count);
        Assert.Contains(streams, s => s.Id == user1.Id);
        Assert.Contains(streams, s => s.Id == user3.Id);
        break;
      case true:
        Assert.Equal(2, streams.Count);
        Assert.Contains(streams, s => s.Id == user2.Id);
        Assert.Contains(streams, s => s.Id == user3.Id);
        break;
      default:
        Assert.Equal(3, streams.Count);
        Assert.Contains(streams, s => s.Id == user1.Id);
        Assert.Contains(streams, s => s.Id == user2.Id);
        Assert.Contains(streams, s => s.Id == user3.Id);
        break;
    }
  }

  [Theory(DisplayName = "FetchAsync: it should return null when no stream was found.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoStream_When_FetchAsync_Then_Null(bool isDeleted)
  {
    if (isDeleted)
    {
      User user = new(_faker.Person.UserName);
      user.Delete();

      StreamEntity entity = new(user.Id, user.GetType());
      foreach (IEvent change in user.Changes)
      {
        entity.Append(_converter.ToEventEntity(change, entity));
      }
      _context.Streams.Add(entity);
      _context.SaveChanges();
    }

    Stream? stream = await _store.FetchAsync(StreamId.NewId(), new FetchOptions
    {
      IsDeleted = isDeleted ? false : null
    }, _cancellationToken);
    Assert.Null(stream);
  }

  [Fact(DisplayName = "FetchAsync: it should return the stream found.")]
  public async Task Given_StreamFound_When_FetchAsync_Then_Returned()
  {
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1
    };
    StreamEntity entity = new(created.StreamId, typeof(User));
    EventEntity eventEntity = _converter.ToEventEntity(created, entity);
    entity.Append(eventEntity);
    _context.Streams.Add(entity);
    _context.SaveChanges();

    Stream? stream = await _store.FetchAsync(created.StreamId, options: null, _cancellationToken);

    Assert.NotNull(stream);
    Assert.Equal(created.StreamId, stream.Id);
    Assert.Equal(typeof(User), stream.Type);
    Assert.Equal(created.Version, stream.Version);
    Assert.Equal(created.ActorId, stream.CreatedBy);
    Assert.Equal(created.OccurredOn.AsUniversalTime(), stream.CreatedOn);
    Assert.Equal(created.ActorId, stream.UpdatedBy);
    Assert.Equal(created.OccurredOn.AsUniversalTime(), stream.UpdatedOn);
    Assert.Equal(created.IsDeleted ?? false, stream.IsDeleted);

    Event @event = Assert.Single(stream.Events);
    Assert.Equal(created.Id, @event.Id);
    Assert.Equal(created.Version, @event.Version);
    Assert.Equal(created.ActorId, @event.ActorId);
    Assert.Equal(created.OccurredOn.AsUniversalTime(), @event.OccurredOn);
    Assert.Equal(created.IsDeleted, @event.IsDeleted);
    Assert.Equal(created, @event.Data);
  }

  [Fact(DisplayName = "EnforceStreamExpectation: it should throw InvalidOperationException when the stream does not exist but is expected to exist.")]
  public async Task Given_NoStreamShouldExist_When_EnforceStreamExpectation_Then_InvalidOperationException()
  {
    UserSignedIn signedIn = new()
    {
      StreamId = StreamId.NewId(),
      Version = 2
    };
    _store.Append(signedIn.StreamId, typeof(User), StreamExpectation.ShouldExist, [signedIn]);

    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _store.SaveChangesAsync(_cancellationToken));
    Assert.Equal(string.Format("The stream 'Id={0}' was expected to exist, but does not exist.", signedIn.StreamId), exception.Message);
  }

  [Fact(DisplayName = "EnforceStreamExpectation: it should throw InvalidOperationException when the stream exists but is expected to not exist.")]
  public async Task Given_StreamFoundShouldNotExist_When_EnforceStreamExpectation_Then_InvalidOperationException()
  {
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1
    };
    _store.Append(created.StreamId, typeof(User), StreamExpectation.ShouldNotExist, [created]);

    StreamEntity stream = new(created.StreamId, typeof(User));
    EventEntity @event = new(created.Id, stream, created.OccurredOn, created.GetType().Name, created.GetType().GetNamespaceQualifiedName(), _serializer.Serialize(created), created.ActorId, created.IsDeleted);
    stream.Append(@event);
    _context.Streams.Add(stream);
    _context.SaveChanges();

    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _store.SaveChangesAsync(_cancellationToken));
    Assert.Equal(string.Format("The stream 'Id={0}' was not expected to exist, but was found at version {1}.", created.StreamId, stream.Version), exception.Message);
  }

  [Theory(DisplayName = "EnforceStreamExpectation: it should throw InvalidOperationException when the stream was found at an unexpected version.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_StreamVersionNotExpected_When_EnforceStreamExpectation_Then_InvalidOperationException(bool exists)
  {
    UserSignedIn signedIn = new()
    {
      StreamId = StreamId.NewId(),
      Version = 2
    };
    _store.Append(signedIn.StreamId, typeof(User), StreamExpectation.ShouldBeAtVersion(signedIn.Version), [signedIn]);

    StreamEntity? stream = null;
    if (exists)
    {
      stream = new StreamEntity(signedIn.StreamId, typeof(User));

      UserCreated created = new(_faker.Person.UserName)
      {
        StreamId = signedIn.StreamId,
        Version = 1,
        OccurredOn = DateTime.Now.AddDays(-1)
      };
      EventEntity event1 = new(created.Id, stream, created.OccurredOn, created.GetType().Name, created.GetType().GetNamespaceQualifiedName(), _serializer.Serialize(created), created.ActorId, created.IsDeleted);
      stream.Append(event1);

      UserPasswordCreated password = new(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")))
      {
        StreamId = signedIn.StreamId,
        Version = 2,
        OccurredOn = DateTime.Now.AddHours(-1)
      };
      EventEntity event2 = new(password.Id, stream, password.OccurredOn, password.GetType().Name, password.GetType().GetNamespaceQualifiedName(), _serializer.Serialize(password), password.ActorId, password.IsDeleted);
      stream.Append(event2);

      _context.Streams.Add(stream);
      _context.SaveChanges();
    }

    var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _store.SaveChangesAsync(_cancellationToken));
    Assert.Equal(string.Format(
      "The stream 'Id={0}' was expected to be at version {1}, but was found at version {2}.",
      signedIn.StreamId,
      signedIn.Version - 1,
      stream?.Version ?? 0), exception.Message);
  }

  [Theory(DisplayName = "SaveChangesAsync: it should create a new stream when it does not exist.")]
  [InlineData(StreamExpectation.StreamExpectationKind.None)]
  [InlineData(StreamExpectation.StreamExpectationKind.ShouldBeAtVersion)]
  [InlineData(StreamExpectation.StreamExpectationKind.ShouldNotExist)]
  public async Task Given_NewStream_When_SaveChangesAsync_Then_ItIsCreated(StreamExpectation.StreamExpectationKind kind)
  {
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      ActorId = ActorId.NewId(),
      OccurredOn = DateTime.Now.AddDays(-1),
      IsDeleted = false
    };
    StreamExpectation expectation = kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion ? new(created.Version) : new(kind);
    _store.Append(created.StreamId, typeof(User), expectation, [created]);

    await _store.SaveChangesAsync(_cancellationToken);

    StreamEntity? stream = _context.Streams.AsNoTracking()
      .Include(x => x.Events)
      .SingleOrDefault(x => x.Id == created.StreamId.Value);
    Assert.NotNull(stream);
    Assert.Equal(created.StreamId.Value, stream.Id);
    Assert.Equal(typeof(User).GetNamespaceQualifiedName(), stream.Type);
    Assert.Equal(created.Version, stream.Version);
    Assert.Equal(created.ActorId?.Value, stream.CreatedBy);
    Assert.Equal(created.OccurredOn.AsUniversalTime(), stream.CreatedOn);
    Assert.Equal(created.ActorId?.Value, stream.UpdatedBy);
    Assert.Equal(created.OccurredOn.AsUniversalTime(), stream.UpdatedOn);
    Assert.Equal(created.IsDeleted, stream.IsDeleted);

    EventEntity @event = Assert.Single(stream.Events);
    Assert.Equal(created.Id.Value, @event.Id);
    Assert.Same(stream, @event.Stream);
    Assert.Equal(stream.StreamId, @event.StreamId);
    Assert.Equal(created.Version, @event.Version);
    Assert.Equal(created.ActorId?.Value, @event.ActorId);
    Assert.Equal(created.OccurredOn.AsUniversalTime(), @event.OccurredOn);
    Assert.Equal(created.IsDeleted, @event.IsDeleted);
    Assert.Equal(created.GetType().Name, @event.TypeName);
    Assert.Equal(created.GetType().GetNamespaceQualifiedName(), @event.NamespacedType);
    Assert.Equal(_serializer.Serialize(created), @event.Data);

    Assert.False(_store.HasChanges);

    _bus.Verify(x => x.PublishAsync(created, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "SaveChangesAsync: it should update an existing stream.")]
  [InlineData(StreamExpectation.StreamExpectationKind.None)]
  [InlineData(StreamExpectation.StreamExpectationKind.ShouldBeAtVersion)]
  [InlineData(StreamExpectation.StreamExpectationKind.ShouldExist)]
  public async Task Given_ExistingStream_When_SaveChangesAsync_Then_ItIsUpdated(StreamExpectation.StreamExpectationKind kind)
  {
    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      ActorId = ActorId.NewId(),
      OccurredOn = DateTime.Now.AddDays(-1),
      IsDeleted = false
    };
    StreamEntity stream = new(created.StreamId, typeof(User));
    EventEntity @event = new(created.Id, stream, created.OccurredOn, created.GetType().Name, created.GetType().GetNamespaceQualifiedName(), _serializer.Serialize(created), created.ActorId, created.IsDeleted);
    stream.Append(@event);
    _context.Streams.Add(stream);
    _context.SaveChanges();

    UserSignedIn signedIn = new()
    {
      StreamId = created.StreamId,
      Version = 2,
      ActorId = new ActorId(created.ActorId.Value.ToGuid())
    };
    StreamExpectation expectation = kind == StreamExpectation.StreamExpectationKind.ShouldBeAtVersion ? new(signedIn.Version) : new(kind);
    _store.Append(created.StreamId, typeof(User), expectation, [signedIn]);

    await _store.SaveChangesAsync(_cancellationToken);

    StreamEntity? updatedStream = _context.Streams.AsNoTracking()
      .Include(x => x.Events)
      .SingleOrDefault(x => x.Id == created.StreamId.Value);
    Assert.NotNull(updatedStream);
    Assert.Equal(stream.Id, updatedStream.Id);
    Assert.Equal(stream.Type, updatedStream.Type);
    Assert.Equal(signedIn.Version, updatedStream.Version);
    Assert.Equal(stream.CreatedBy, updatedStream.CreatedBy);
    Assert.Equal(stream.CreatedOn, updatedStream.CreatedOn);
    Assert.Equal(signedIn.ActorId?.Value, updatedStream.UpdatedBy);
    Assert.Equal(signedIn.OccurredOn.AsUniversalTime(), updatedStream.UpdatedOn);
    Assert.Equal(signedIn.IsDeleted ?? stream.IsDeleted, updatedStream.IsDeleted);

    Assert.Equal(2, stream.Events.Count);
    EventEntity newEvent = Assert.Single(stream.Events.Skip(1));
    Assert.Equal(signedIn.Id.Value, newEvent.Id);
    Assert.Same(stream, newEvent.Stream);
    Assert.Equal(stream.StreamId, newEvent.StreamId);
    Assert.Equal(signedIn.Version, newEvent.Version);
    Assert.Equal(signedIn.ActorId?.Value, newEvent.ActorId);
    Assert.Equal(signedIn.OccurredOn.AsUniversalTime(), newEvent.OccurredOn);
    Assert.Equal(signedIn.IsDeleted, newEvent.IsDeleted);
    Assert.Equal(signedIn.GetType().Name, newEvent.TypeName);
    Assert.Equal(signedIn.GetType().GetNamespaceQualifiedName(), newEvent.NamespacedType);
    Assert.Equal(_serializer.Serialize(signedIn), newEvent.Data);

    Assert.False(_store.HasChanges);

    _bus.Verify(x => x.PublishAsync(signedIn, _cancellationToken), Times.Once);
  }
}
