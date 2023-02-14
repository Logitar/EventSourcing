using Logitar.EventSourcing.Demo.Domain;
using Logitar.EventSourcing.Demo.ReadModel.Entities;
using MediatR;

namespace Logitar.EventSourcing.Demo.ReadModel.Handlers;

internal class TodoCreatedHandler : INotificationHandler<TodoCreated>
{
  private readonly TodoContext _context;
  private readonly ILogger<TodoCreatedHandler> _logger;

  public TodoCreatedHandler(TodoContext context, ILogger<TodoCreatedHandler> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(TodoCreated notification, CancellationToken cancellationToken)
  {
    try
    {
      TodoEntity todo = new(notification);

      _context.Todos.Add(todo);
      await _context.SaveChangesAsync(cancellationToken);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "An unexpected error occurred.");
    }
  }
}
