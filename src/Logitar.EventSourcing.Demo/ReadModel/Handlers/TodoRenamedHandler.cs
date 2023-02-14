using Logitar.EventSourcing.Demo.Domain;
using Logitar.EventSourcing.Demo.ReadModel.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.ReadModel.Handlers;

internal class TodoRenamedHandler : INotificationHandler<TodoRenamed>
{
  private readonly TodoContext _context;
  private readonly ILogger<TodoRenamedHandler> _logger;

  public TodoRenamedHandler(TodoContext context, ILogger<TodoRenamedHandler> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(TodoRenamed notification, CancellationToken cancellationToken)
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

      todo.Rename(notification);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "An unexpected error occurred.");
    }
  }
}
