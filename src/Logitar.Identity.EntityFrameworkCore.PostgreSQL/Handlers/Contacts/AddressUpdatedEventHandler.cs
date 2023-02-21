using Logitar.Identity.Contacts.Events;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Handlers.Contacts;

/// <summary>
/// The handler for <see cref="AddressUpdatedEvent"/> events.
/// </summary>
internal class AddressUpdatedEventHandler : INotificationHandler<AddressUpdatedEvent>
{
  /// <summary>
  /// The identity database context.
  /// </summary>
  private readonly IdentityContext _context;
  /// <summary>
  /// The logger instance.
  /// </summary>
  private readonly ILogger<AddressUpdatedEventHandler> _logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="AddressUpdatedEventHandler"/> class with the specified arguments.
  /// </summary>
  /// <param name="context">The identity database context.</param>
  /// <param name="logger">The logger instance.</param>
  public AddressUpdatedEventHandler(IdentityContext context, ILogger<AddressUpdatedEventHandler> logger)
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
  public async Task Handle(AddressUpdatedEvent notification, CancellationToken cancellationToken)
  {
    try
    {
      AddressEntity? address = await _context.Addresses
        .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);
      if (address == null)
      {
        _logger.LogError("The postal address 'AggregateId={id}' could not be found.", notification.AggregateId);
        return;
      }

      address.Update(notification);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception);
    }
  }
}
