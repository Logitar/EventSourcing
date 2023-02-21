using Logitar.Identity.Contacts.Events;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Handlers.Contacts;

/// <summary>
/// The handler for <see cref="PhoneCreatedEvent"/> events.
/// </summary>
internal class PhoneCreatedEventHandler : INotificationHandler<PhoneCreatedEvent>
{
  /// <summary>
  /// The identity database context.
  /// </summary>
  private readonly IdentityContext _context;
  /// <summary>
  /// The logger instance.
  /// </summary>
  private readonly ILogger<PhoneCreatedEventHandler> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneCreatedEventHandler"/> class with the specified arguments.
  /// </summary>
  /// <param name="context">The identity database context.</param>
  /// <param name="logger">The logger instance.</param>
  public PhoneCreatedEventHandler(IdentityContext context, ILogger<PhoneCreatedEventHandler> logger)
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
  public async Task Handle(PhoneCreatedEvent notification, CancellationToken cancellationToken)
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

      PhoneEntity phone = new(notification, user);

      _context.Phones.Add(phone);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
    }
  }
}
