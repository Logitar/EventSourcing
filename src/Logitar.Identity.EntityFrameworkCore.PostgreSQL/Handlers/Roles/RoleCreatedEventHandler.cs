using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Roles.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Handlers.Roles;

/// <summary>
/// The handler for <see cref="RoleCreatedEvent"/> events.
/// </summary>
internal class RoleCreatedEventHandler : INotificationHandler<RoleCreatedEvent>
{
  /// <summary>
  /// The identity database context.
  /// </summary>
  private readonly IdentityContext _context;
  /// <summary>
  /// The logger instance.
  /// </summary>
  private readonly ILogger<RoleCreatedEventHandler> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="RoleCreatedEventHandler"/> class with the specified arguments.
  /// </summary>
  /// <param name="context">The identity database context.</param>
  /// <param name="logger">The logger instance.</param>
  public RoleCreatedEventHandler(IdentityContext context, ILogger<RoleCreatedEventHandler> logger)
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
  public async Task Handle(RoleCreatedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      RealmEntity? realm = await _context.Realms
        .SingleOrDefaultAsync(x => x.AggregateId == notification.RealmId.Value, cancellationToken);
      if (realm == null)
      {
        _logger.LogError("The realm 'AggregateId={id}' could not be found.", notification.RealmId);
        return;
      }

      RoleEntity role = new(notification, realm);

      _context.Roles.Add(role);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
    }
  }
}
