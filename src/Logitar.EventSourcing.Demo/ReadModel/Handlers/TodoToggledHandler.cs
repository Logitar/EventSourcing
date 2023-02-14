using Logitar.EventSourcing.Demo.Domain;
using Logitar.EventSourcing.Demo.ReadModel.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.ReadModel.Handlers;

internal class TodoToggledHandler : INotificationHandler<TodoToggled>
{
  private readonly TodoContext _context;
  private readonly ILogger<TodoToggledHandler> _logger;

  public TodoToggledHandler(TodoContext context, ILogger<TodoToggledHandler> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(TodoToggled notification, CancellationToken cancellationToken)
  {
    try
    {
      TodoEntity? todo = await _context.Todos
        .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);
      if (todo == null)
      {
        _logger.LogError("The specified todo (AggregateId={id}) could not be found.", notification.AggregateId);
        return;
      }

      todo.Toggle(notification);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "An unexpected error occurred.");
    }
  }
}
