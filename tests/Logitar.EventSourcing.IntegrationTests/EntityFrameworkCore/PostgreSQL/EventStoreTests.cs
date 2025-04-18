﻿using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

[Trait(Traits.Category, Categories.Integration)]
public class EventStoreTests : EntityFrameworkCoreIntegrationTests
{
  private readonly IEventStore _store;

  public EventStoreTests() : base(DatabaseProvider.PostgreSQL)
  {
    _store = ServiceProvider.GetRequiredService<IEventStore>();
  }

  [Fact(DisplayName = "FetchAsync: it should return empty when no event was found (many).")]
  public async Task Given_NoStream_When_FetchManyAsync_Then_EmptyReturned()
  {
    User user = new(Faker.Person.UserName);
    StreamEntity entity = new(user.Id, user.GetType());
    foreach (IEvent change in user.Changes)
    {
      entity.Append(EventConverter.ToEventEntity(change, entity));
    }
    EventContext.Streams.Add(entity);
    await EventContext.SaveChangesAsync(CancellationToken);

    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(new FetchManyOptions
    {
      StreamTypes = [typeof(Session)]
    }, CancellationToken);
    Assert.Empty(streams);
  }

  [Fact(DisplayName = "FetchAsync: it should return null when no event was found (single).")]
  public async Task Given_NoStream_When_FetchSingleAsync_Then_NullReturned()
  {
    User user = new(Faker.Person.UserName);
    StreamEntity entity = new(user.Id, user.GetType());
    foreach (IEvent change in user.Changes)
    {
      entity.Append(EventConverter.ToEventEntity(change, entity));
    }
    EventContext.Streams.Add(entity);
    await EventContext.SaveChangesAsync(CancellationToken);

    Stream? stream = await _store.FetchAsync(user.Id, new FetchOptions
    {
      FromVersion = user.Version + 1
    }, CancellationToken);
    Assert.Null(stream);
  }

  [Fact(DisplayName = "FetchAsync: it should return the correct stream when events were found (single).")]
  public async Task Given_StreamsAndEvents_When_FetchSingleAsync_Then_CorrectStreamReturned()
  {
    User user = new(Faker.Person.UserName);

    user.SignIn(new ActorId(user.Id.Value));
    UserSignedIn? signedIn = user.Changes.Last() as UserSignedIn;
    Assert.NotNull(signedIn);

    user.Disable(new ActorId(Guid.Empty));
    UserDisabled? disabled = user.Changes.Last() as UserDisabled;
    Assert.NotNull(disabled);

    user.Delete(new ActorId("SYSTEM"));
    StreamEntity entity = new(user.Id, user.GetType());
    foreach (IEvent change in user.Changes)
    {
      entity.Append(EventConverter.ToEventEntity(change, entity));
    }
    EventContext.Streams.Add(entity);
    await EventContext.SaveChangesAsync(CancellationToken);

    Stream? stream = await _store.FetchAsync(user.Id, new FetchOptions
    {
      FromVersion = 2,
      ToVersion = 3
    }, CancellationToken);
    Assert.NotNull(stream);

    Assert.Equal(user.Id, stream.Id);
    Assert.Equal(typeof(User), stream.Type);
    Assert.Equal(3, stream.Version);
    Assert.Null(stream.CreatedBy);
    Assert.Null(stream.CreatedOn);
    Assert.Equal(disabled.ActorId, stream.UpdatedBy);
    Assert.True(stream.UpdatedOn.HasValue);
    Assert.Equal(disabled.OccurredOn.AsUniversalTime(), stream.UpdatedOn.Value, TimeSpan.FromSeconds(1));
    Assert.False(stream.IsDeleted);

    Assert.Equal(2, stream.Events.Count);
    Assert.Contains(stream.Events, e => e.Id == signedIn.Id && e.Version == signedIn.Version && e.ActorId == signedIn.ActorId
      && (signedIn.OccurredOn - e.OccurredOn.AsUniversalTime() < TimeSpan.FromSeconds(1)) && e.IsDeleted == signedIn.IsDeleted && e.Data.Equals(signedIn));
    Assert.Contains(stream.Events, e => e.Id == disabled.Id && e.Version == disabled.Version && e.ActorId == disabled.ActorId
      && (disabled.OccurredOn - e.OccurredOn.AsUniversalTime() < TimeSpan.FromSeconds(1)) && e.IsDeleted == null && e.Data.Equals(disabled));
  }

  [Fact(DisplayName = "FetchAsync: it should return the correct streams when events were found (many).")]
  public async Task Given_StreamsAndEvents_When_FetchManyAsync_Then_CorrectStreamReturned()
  {
    UserCreated userCreated = new(Faker.Person.UserName)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddDays(-1)
    };
    ActorId actorId = new(userCreated.StreamId.Value);
    userCreated.ActorId = actorId;
    UserSignedIn userSignedIn = new()
    {
      StreamId = userCreated.StreamId,
      Version = 2,
      OccurredOn = DateTime.Now.AddHours(-1),
      ActorId = actorId
    };
    StreamEntity userStream = new(userCreated.StreamId, typeof(User));
    userStream.Append(EventConverter.ToEventEntity(userCreated, userStream));
    userStream.Append(EventConverter.ToEventEntity(userSignedIn, userStream));

    SessionCreated sessionCreated = new(userCreated.StreamId)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddHours(-1),
      ActorId = actorId
    };
    StreamEntity sessionStream = new(sessionCreated.StreamId, typeof(Session));
    sessionStream.Append(EventConverter.ToEventEntity(sessionCreated, sessionStream));

    UserDeleted userDeleted = new()
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddDays(-10),
      ActorId = actorId,
      IsDeleted = true
    };
    StreamEntity deletedUserStream = new(userDeleted.StreamId, typeof(User));
    deletedUserStream.Append(EventConverter.ToEventEntity(userDeleted, deletedUserStream));

    RoleCreated roleCreated = new("admin")
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddDays(-20),
      ActorId = actorId
    };
    StreamEntity roleStream = new(roleCreated.StreamId, typeof(Role));
    roleStream.Append(EventConverter.ToEventEntity(roleCreated, roleStream));

    UserCreated oldUserCreated = new(Faker.Internet.UserName())
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddYears(-1),
      ActorId = actorId
    };
    StreamEntity oldUserStream = new(oldUserCreated.StreamId, typeof(User));
    oldUserStream.Append(EventConverter.ToEventEntity(oldUserCreated, oldUserStream));

    UserCreated newUserCreated = new(Faker.Internet.UserName())
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now,
      ActorId = actorId
    };
    StreamEntity newUserStream = new(newUserCreated.StreamId, typeof(User));
    newUserStream.Append(EventConverter.ToEventEntity(newUserCreated, newUserStream));

    SessionCreated systemSession = new(userCreated.StreamId)
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddDays(-3)
    };
    StreamEntity systemSessionStream = new(systemSession.StreamId, typeof(Session));
    systemSessionStream.Append(EventConverter.ToEventEntity(systemSession, systemSessionStream));

    UserCreated unwantedUserCreated = new(Faker.Internet.UserName())
    {
      StreamId = StreamId.NewId(),
      Version = 1,
      OccurredOn = DateTime.Now.AddMinutes(-10),
      ActorId = actorId
    };
    StreamEntity unwantedUserStream = new(unwantedUserCreated.StreamId, typeof(User));
    unwantedUserStream.Append(EventConverter.ToEventEntity(unwantedUserCreated, unwantedUserStream));

    EventContext.Streams.AddRange(userStream, sessionStream, deletedUserStream, roleStream, oldUserStream, newUserStream, systemSessionStream, unwantedUserStream);
    await EventContext.SaveChangesAsync(CancellationToken);

    FetchManyOptions options = new()
    {
      StreamTypes = [typeof(User), typeof(Session)],
      StreamIds =
      [
        userCreated.StreamId,
        sessionCreated.StreamId,
        userDeleted.StreamId,
        roleCreated.StreamId,
        oldUserCreated.StreamId,
        newUserCreated.StreamId,
        systemSession.StreamId,
        StreamId.NewId(),
        new StreamId()
      ],
      FromVersion = 1,
      ToVersion = 1,
      OccurredFrom = DateTime.Now.AddMonths(-1),
      OccurredTo = DateTime.Now.AddMinutes(-1),
      Actor = new ActorFilter(actorId),
      IsDeleted = false
    };
    IReadOnlyCollection<Stream> streams = await _store.FetchAsync(options, CancellationToken);

    Assert.Equal(2, streams.Count);
    Assert.Contains(streams, s => s.Id == userCreated.StreamId && typeof(User).Equals(s.Type) && s.Version == 1 && s.CreatedBy == actorId && s.CreatedOn.HasValue
      && s.UpdatedBy == s.CreatedBy && s.UpdatedOn == s.CreatedOn && !s.IsDeleted && Assert.Single(s.Events).Id == userCreated.Id);
    Assert.Contains(streams, s => s.Id == sessionCreated.StreamId && typeof(Session).Equals(s.Type) && s.Version == 1 && s.CreatedBy == actorId
      && s.CreatedOn.HasValue && s.UpdatedBy == s.CreatedBy && s.UpdatedOn == s.CreatedOn && Assert.Single(s.Events).Id == sessionCreated.Id);
  }

  [Fact(DisplayName = "SaveChangesAsync: it should commit the changes to streams and events to the database.")]
  public async Task Given_UncommittedChanges_When_SaveChangesAsync_Then_StreamsAndEventsCommitted()
  {
    User user1 = new(Faker.Person.UserName);
    StreamEntity stream = new(user1.Id, user1.GetType());
    foreach (IEvent change in user1.Changes)
    {
      stream.Append(EventConverter.ToEventEntity(change, stream));
    }
    EventContext.Streams.Add(stream);
    await EventContext.SaveChangesAsync(CancellationToken);

    user1.ClearChanges();
    user1.SignIn(new ActorId(user1.Id.Value));
    user1.Disable(new ActorId("SYSTEM"));
    _store.Append(user1.Id, user1.GetType(), StreamExpectation.ShouldBeAtVersion(user1.Version), user1.Changes);

    User user2 = new(Faker.Internet.UserName());
    user2.Delete(new ActorId(Guid.Empty));
    _store.Append(user2.Id, user2.GetType(), StreamExpectation.ShouldNotExist, user2.Changes);

    await _store.SaveChangesAsync(CancellationToken);

    StreamEntity? stream1 = await EventContext.Streams.AsNoTracking()
      .Include(x => x.Events)
      .SingleOrDefaultAsync(x => x.Id == user1.Id.Value, CancellationToken);
    Assert.NotNull(stream1);
    AssertEqual(user1, stream1);

    StreamEntity? stream2 = await EventContext.Streams.AsNoTracking()
      .Include(x => x.Events)
      .SingleOrDefaultAsync(x => x.Id == user2.Id.Value, CancellationToken);
    Assert.NotNull(stream2);
    AssertEqual(user2, stream2);

    Assert.False(_store.HasChanges);
  }

  private static void AssertEqual(AggregateRoot aggregate, StreamEntity stream)
  {
    Assert.Equal(aggregate.Id.Value, stream.Id);
    Assert.Equal(aggregate.GetType().GetNamespaceQualifiedName(), stream.Type);
    Assert.Equal(aggregate.Version, stream.Version);
    Assert.Equal(aggregate.CreatedBy?.Value, stream.CreatedBy);
    Assert.Equal(aggregate.CreatedOn.AsUniversalTime(), stream.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(aggregate.UpdatedBy?.Value, stream.UpdatedBy);
    Assert.Equal(aggregate.UpdatedOn.AsUniversalTime(), stream.UpdatedOn, TimeSpan.FromSeconds(1));
    Assert.Equal(aggregate.IsDeleted, stream.IsDeleted);
    Assert.Equal(aggregate.Version, stream.Events.Count);
  }
}
