using Logitar.Identity.Contacts.Events;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Handlers.Contacts;

/// <summary>
/// The handler for <see cref="PhoneDeletedEvent"/> events.
/// </summary>
internal class PhoneDeletedEventHandler : INotificationHandler<PhoneDeletedEvent>
{
  /// <summary>
  /// The identity database context.
  /// </summary>
  private readonly IdentityContext _context;
  /// <summary>
  /// The logger instance.
  /// </summary>
  private readonly ILogger<PhoneDeletedEventHandler> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneDeletedEventHandler"/> class with the specified arguments.
  /// </summary>
  /// <param name="context">The identity database context.</param>
  /// <param name="logger">The logger instance.</param>
  public PhoneDeletedEventHandler(IdentityContext context, ILogger<PhoneDeletedEventHandler> logger)
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
  public async Task Handle(PhoneDeletedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      PhoneEntity? phone = await _context.Phones
        .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);
      if (phone == null)
      {
        _logger.LogError("The phone number 'AggregateId={id}' could not be found.", notification.AggregateId);
        return;
      }

      _context.Phones.Remove(phone);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
    }
  }
}
