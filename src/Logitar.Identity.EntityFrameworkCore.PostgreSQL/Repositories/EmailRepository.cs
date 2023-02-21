using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Contacts;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Repositories;

/// <summary>
/// Implements methods to load email addresses from the event store.
/// </summary>
internal class EmailRepository : EventStore, IEmailRepository
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EmailRepository"/> class using the specified arguments.
  /// </summary>
  /// <param name="context">The event database context.</param>
  /// <param name="eventBus">The event bus.</param>
  public EmailRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  /// <summary>
  /// Retrieves the list of email addresses of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the email addresses.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of email addresses, or empty if none.</returns>
  public async Task<IEnumerable<EmailAggregate>> LoadByUserAsync(AggregateId userId, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(EmailAggregate).GetName();

    EventEntity[] events = await Context.Events.FromSql($@"SELECT e.* FROM ""Events"" e JOIN ""Emails"" a on a.""AggregateId"" = e.""AggregateId"" JOIN ""Users"" u ON u.""UserId"" = a.""UserId"" WHERE e.""AggregateType"" = {aggregateType} AND u.""AggregateId"" = {userId.Value}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<EmailAggregate>(events.GroupBy(e => new AggregateId(e.AggregateId)));
  }

  /// <summary>
  /// Retrieves the default email address of the specified user.
  /// </summary>
  /// <param name="userId">The identifier of the user to retrieve the default email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The default email address or null if not found.</returns>
  public async Task<EmailAggregate?> LoadDefaultAsync(AggregateId userId, CancellationToken cancellationToken)
  {
    string aggregateType = typeof(EmailAggregate).GetName();

    EventEntity[] events = await Context.Events.FromSql($@"SELECT e.* FROM ""Events"" e JOIN ""Emails"" a on a.""AggregateId"" = e.""AggregateId"" JOIN ""Users"" u ON u.""UserId"" = a.""UserId"" WHERE e.""AggregateType"" = {aggregateType} AND a.""IsDefault"" = true AND u.""AggregateId"" = {userId.Value}")
      .AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    return Load<EmailAggregate>(events);
  }
}
