using Logitar.Identity.Contacts.Events;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Handlers.Contacts;

/// <summary>
/// The handler for <see cref="EmailDeletedEvent"/> events.
/// </summary>
internal class EmailDeletedEventHandler : INotificationHandler<EmailDeletedEvent>
{
  /// <summary>
  /// The identity database context.
  /// </summary>
  private readonly IdentityContext _context;
  /// <summary>
  /// The logger instance.
  /// </summary>
  private readonly ILogger<EmailDeletedEventHandler> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="EmailDeletedEventHandler"/> class with the specified arguments.
  /// </summary>
  /// <param name="context">The identity database context.</param>
  /// <param name="logger">The logger instance.</param>
  public EmailDeletedEventHandler(IdentityContext context, ILogger<EmailDeletedEventHandler> logger)
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
  public async Task Handle(EmailDeletedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      EmailEntity? email = await _context.Emails
        .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);
      if (email == null)
      {
        _logger.LogError("The email address 'AggregateId={id}' could not be found.", notification.AggregateId);
        return;
      }

      _context.Emails.Remove(email);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
    }
  }
}
