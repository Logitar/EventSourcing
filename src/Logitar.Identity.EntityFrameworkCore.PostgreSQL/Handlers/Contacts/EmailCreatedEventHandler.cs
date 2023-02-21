using Logitar.Identity.Contacts.Events;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Handlers.Contacts;

/// <summary>
/// The handler for <see cref="EmailCreatedEvent"/> events.
/// </summary>
internal class EmailCreatedEventHandler : INotificationHandler<EmailCreatedEvent>
{
  /// <summary>
  /// The identity database context.
  /// </summary>
  private readonly IdentityContext _context;
  /// <summary>
  /// The logger instance.
  /// </summary>
  private readonly ILogger<EmailCreatedEventHandler> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="EmailCreatedEventHandler"/> class with the specified arguments.
  /// </summary>
  /// <param name="context">The identity database context.</param>
  /// <param name="logger">The logger instance.</param>
  public EmailCreatedEventHandler(IdentityContext context, ILogger<EmailCreatedEventHandler> logger)
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
  public async Task Handle(EmailCreatedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      UserEntity? user = await _context.Users
        .SingleOrDefaultAsync(x => x.AggregateId == notification.UserId.Value, cancellationToken);
      if (user == null)
      {
        _logger.LogError("The user 'AggregateId={id}' could not be found.", notification.UserId);
        return;
      }

      EmailEntity email = new(notification, user);

      _context.Emails.Add(email);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
    }
  }
}
