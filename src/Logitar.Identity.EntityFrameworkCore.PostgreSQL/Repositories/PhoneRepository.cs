using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Contacts;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Repositories;

/// <summary>
/// Implements methods to load phone numbers from the event store.
/// </summary>
internal class PhoneRepository : EventStore, IPhoneRepository
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneRepository"/> class using the specified arguments.
  /// </summary>
  /// <param name="context">The event database context.</param>
  /// <param name="eventBus">The event bus.</param>
  public PhoneRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  /// <summary>
  /// Retrieves the list of phone numbers of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the phone numbers.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of phone numbers, or empty if none.</returns>
  public async Task<IEnumerable<PhoneAggregate>> LoadByUserAsync(AggregateId userId, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(PhoneAggregate).GetName();

    EventEntity[] events = await Context.Events.FromSql($@"SELECT e.* FROM ""Events"" e JOIN ""Phones"" p on p.""AggregateId"" = e.""AggregateId"" JOIN ""Users"" u ON u.""UserId"" = p.""UserId"" WHERE e.""AggregateType"" = {aggregateType} AND u.""AggregateId"" = {userId.Value}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<PhoneAggregate>(events.GroupBy(e => new AggregateId(e.AggregateId)));
  }

  /// <summary>
  /// Retrieves the default phone number of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the default phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The default phone number or null if not found.</returns>
  public async Task<PhoneAggregate?> LoadDefaultAsync(AggregateId userId, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(PhoneAggregate).GetName();

    EventEntity[] events = await Context.Events.FromSql($@"SELECT e.* FROM ""Events"" e JOIN ""Phones"" p on p.""AggregateId"" = e.""AggregateId"" JOIN ""Users"" u ON u.""UserId"" = p.""UserId"" WHERE e.""AggregateType"" = {aggregateType} AND p.""IsDefault"" = true AND u.""AggregateId"" = {userId.Value}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<PhoneAggregate>(events);
  }
}
