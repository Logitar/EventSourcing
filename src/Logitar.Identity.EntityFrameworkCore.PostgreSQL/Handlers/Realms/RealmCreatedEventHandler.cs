using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Realms.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Handlers.Realms;

/// <summary>
/// The handler for <see cref="RealmCreatedEvent"/> events.
/// </summary>
internal class RealmCreatedEventHandler : INotificationHandler<RealmCreatedEvent>
{
  /// <summary>
  /// The identity database context.
  /// </summary>
  private readonly IdentityContext _context;
  /// <summary>
  /// The logger instance.
  /// </summary>
  private readonly ILogger<RealmCreatedEventHandler> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="RealmCreatedEventHandler"/> class with the specified arguments.
  /// </summary>
  /// <param name="context">The identity database context.</param>
  /// <param name="logger">The logger instance.</param>
  public RealmCreatedEventHandler(IdentityContext context, ILogger<RealmCreatedEventHandler> logger)
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
  public async Task Handle(RealmCreatedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      RealmEntity realm = new(notification);

      _context.Realms.Add(realm);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
    }
  }
}
