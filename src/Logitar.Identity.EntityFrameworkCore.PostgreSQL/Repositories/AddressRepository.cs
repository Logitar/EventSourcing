using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Contacts;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Repositories;

/// <summary>
/// Implements methods to load postal addresses from the event store.
/// </summary>
internal class AddressRepository : EventStore, IAddressRepository
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AddressRepository"/> class using the specified arguments.
  /// </summary>
  /// <param name="context">The event database context.</param>
  /// <param name="eventBus">The event bus.</param>
  public AddressRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  /// <summary>
  /// Retrieves the list of postal addresses of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the postal addresses.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of postal addresses, or empty if none.</returns>
  public async Task<IEnumerable<AddressAggregate>> LoadByUserAsync(AggregateId userId, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(AddressAggregate).GetName();

    EventEntity[] events = await Context.Events.FromSql($@"SELECT e.* FROM ""Events"" e JOIN ""Addresses"" a on a.""AggregateId"" = e.""AggregateId"" JOIN ""Users"" u ON u.""UserId"" = a.""UserId"" WHERE e.""AggregateType"" = {aggregateType} AND u.""AggregateId"" = {userId.Value}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<AddressAggregate>(events.GroupBy(e => new AggregateId(e.AggregateId)));
  }

  /// <summary>
  /// Retrieves the default postal address of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the default postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The default postal address or null if not found.</returns>
  public async Task<AddressAggregate?> LoadDefaultAsync(AggregateId userId, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(AddressAggregate).GetName();

    EventEntity[] events = await Context.Events.FromSql($@"SELECT e.* FROM ""Events"" e JOIN ""Addresses"" a on a.""AggregateId"" = e.""AggregateId"" JOIN ""Users"" u ON u.""UserId"" = a.""UserId"" WHERE e.""AggregateType"" = {aggregateType} AND a.""IsDefault"" = true AND u.""AggregateId"" = {userId.Value}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<AddressAggregate>(events);
  }
}
