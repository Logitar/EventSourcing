using Logitar.EventSourcing.Demo.Domain;
using Logitar.EventSourcing.Demo.ReadModel.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.ReadModel.Handlers;

internal class TodoDeletedHandler : INotificationHandler<TodoDeleted>
{
  private readonly TodoContext _context;
  private readonly ILogger<TodoDeletedHandler> _logger;

  public TodoDeletedHandler(TodoContext context, ILogger<TodoDeletedHandler> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(TodoDeleted notification, CancellationToken cancellationToken)
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

      _context.Todos.Remove(todo);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "An unexpected error occurred.");
    }
  }
}
