using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Roles.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Handlers.Roles;

/// <summary>
/// The handler for <see cref="RoleDeletedEvent"/> events.
/// </summary>
internal class RoleDeletedEventHandler : INotificationHandler<RoleDeletedEvent>
{
  /// <summary>
  /// The identity database context.
  /// </summary>
  private readonly IdentityContext _context;
  /// <summary>
  /// The logger instance.
  /// </summary>
  private readonly ILogger<RoleDeletedEventHandler> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="RoleDeletedEventHandler"/> class with the specified arguments.
  /// </summary>
  /// <param name="context">The identity database context.</param>
  /// <param name="logger">The logger instance.</param>
  public RoleDeletedEventHandler(IdentityContext context, ILogger<RoleDeletedEventHandler> logger)
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
  public async Task Handle(RoleDeletedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      RoleEntity? role = await _context.Roles
        .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);
      if (role == null)
      {
        _logger.LogError("The role 'AggregateId={id}' could not be found.", notification.AggregateId);
        return;
      }

      _context.Roles.Remove(role);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
    }
  }
}
