using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Realms;
using Logitar.Identity.Users;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Repositories;

/// <summary>
/// Implements methods to load users from the event store.
/// </summary>
internal class UserRepository : EventStore, IUserRepository
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UserRepository"/> class using the specified arguments.
  /// </summary>
  /// <param name="context">The event database context.</param>
  /// <param name="eventBus">The event bus.</param>
  public UserRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  /// <summary>
  /// Retrieves an user by its realm and unique name.
  /// </summary>
  /// <param name="realm">The realm of the user.</param>
  /// <param name="username">The unique name of the user.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  public async Task<UserAggregate?> LoadAsync(RealmAggregate realm, string username, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(UserAggregate).GetName();

    EventEntity[] events = await Context.Events.FromSqlRaw($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u on u.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = u.""RealmId"" WHERE e.""AggregateType"" = {aggregateType} AND r.""AggregateId"" = {realm.Id.Value} AND u.""UsernameNormalized"" = {username.ToUpper()}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<UserAggregate>(events);
  }

  /// <summary>
  /// Retrieves an user by its realm and external identifier.
  /// </summary>
  /// <param name="realm">The realm of the user.</param>
  /// <param name="externalKey">The key of an external identifier.</param>
  /// <param name="externalValue">The value of an external identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  public async Task<UserAggregate?> LoadAsync(RealmAggregate realm, string externalKey, string externalValue, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(UserAggregate).GetName();

    EventEntity[] events = await Context.Events.FromSqlRaw($@"SELECT e.* FROM ""Events"" e JOIN ""Users"" u on u.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" r ON r.""RealmId"" = u.""RealmId"" JOIN ""ExternalIdentifiers"" i on i.""UserId"" = u.""UserId"" WHERE e.""AggregateType"" = {aggregateType} AND r.""AggregateId"" = {realm.Id.Value} AND i.""Key"" = {externalKey} AND i.""Value"" = {externalValue}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<UserAggregate>(events);
  }
}
