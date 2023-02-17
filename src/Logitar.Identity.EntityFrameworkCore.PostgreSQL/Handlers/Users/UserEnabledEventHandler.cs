using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Users.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Handlers.Users;

/// <summary>
/// The handler for <see cref="UserEnabledEvent"/> events.
/// </summary>
internal class UserEnabledEventHandler : INotificationHandler<UserEnabledEvent>
{
  /// <summary>
  /// The identity database context.
  /// </summary>
  private readonly IdentityContext _context;
  /// <summary>
  /// The logger instance.
  /// </summary>
  private readonly ILogger<UserEnabledEventHandler> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="UserEnabledEventHandler"/> class with the specified arguments.
  /// </summary>
  /// <param name="context">The identity database context.</param>
  /// <param name="logger">The logger instance.</param>
  public UserEnabledEventHandler(IdentityContext context, ILogger<UserEnabledEventHandler> logger)
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
  public async Task Handle(UserEnabledEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      UserEntity? user = await _context.Users
        .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);
      if (user == null)
      {
        _logger.LogError("The user 'AggregateId={id}' could not be found.", notification.AggregateId);
        return;
      }

      user.Enable(notification);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
    }
  }
}
