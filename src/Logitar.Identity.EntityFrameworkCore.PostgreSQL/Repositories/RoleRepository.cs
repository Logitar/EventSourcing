using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Realms;
using Logitar.Identity.Roles;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Repositories;

/// <summary>
/// Implements methods to save and load roles from the event store.
/// </summary>
internal class RoleRepository : EventStore, IRoleRepository
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RoleRepository"/> class using the specified arguments.
  /// </summary>
  /// <param name="context">The event database context.</param>
  /// <param name="eventBus">The event bus.</param>
  public RoleRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  /// <summary>
  /// Retrieves a role by its realm and unique name.
  /// </summary>
  /// <param name="realm">The realm of the role.</param>
  /// <param name="uniqueName">The unique name of the role.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The role or null if not found.</returns>
  public async Task<RoleAggregate?> LoadAsync(RealmAggregate realm, string uniqueName, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(RoleAggregate).GetName();

    EventEntity[] events = await Context.Events.FromSql($@"SELECT e.* FROM ""Events"" JOIN ""Roles"" r on r.""AggregateId"" = e.""AggregateId"" JOIN ""Realms"" a ON a.""RealmId"" = r.""RealmId"" WHERE e.""AggregateType"" = {aggregateType} AND a.""AggregateId"" = {realm.Id.Value} AND r.""UniqueNameNormalized"" = {uniqueName.ToUpper()}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<RoleAggregate>(events);
  }

  /// <summary>
  /// Saves the specified role in the event store.
  /// </summary>
  /// <param name="role">The role to save.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public async Task SaveAsync(RoleAggregate role, CancellationToken cancellationToken)
  {
    await base.SaveAsync(role, cancellationToken);
  }
}
