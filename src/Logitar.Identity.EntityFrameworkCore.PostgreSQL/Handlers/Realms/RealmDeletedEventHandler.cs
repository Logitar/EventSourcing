using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Realms.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Handlers.Realms;

/// <summary>
/// The handler for <see cref="RealmDeletedEvent"/> events.
/// </summary>
internal class RealmDeletedEventHandler : INotificationHandler<RealmDeletedEvent>
{
  /// <summary>
  /// The identity database context.
  /// </summary>
  private readonly IdentityContext _context;
  /// <summary>
  /// The logger instance.
  /// </summary>
  private readonly ILogger<RealmDeletedEventHandler> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="RealmDeletedEventHandler"/> class with the specified arguments.
  /// </summary>
  /// <param name="context">The identity database context.</param>
  /// <param name="logger">The logger instance.</param>
  public RealmDeletedEventHandler(IdentityContext context, ILogger<RealmDeletedEventHandler> logger)
  {
    _context = context;
    _logger = logger;
  }

  /// <summary>
  /// Handles the specified event.
  /// </summary>
  /// <param name="notification">The event to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public async Task Handle(RealmDeletedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      RealmEntity? realm = await _context.Realms
        .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);
      if (realm == null)
      {
        _logger.LogError("The realm 'AggregateId={id}' could not be found.", notification.AggregateId);
        return;
      }

      _context.Realms.Remove(realm);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
    }
  }
}
