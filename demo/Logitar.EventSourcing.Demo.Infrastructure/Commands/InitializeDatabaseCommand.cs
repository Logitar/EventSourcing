using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.Infrastructure.Commands;

public record InitializeDatabaseCommand : IRequest;

internal class InitializeDatabaseCommandHandler : IRequestHandler<InitializeDatabaseCommand>
{
  private readonly DemoContext _demoContext;
  private readonly IEnumerable<EventContext> _eventContexts;

  public InitializeDatabaseCommandHandler(DemoContext demoContext, IEnumerable<EventContext> eventContexts)
  {
    _demoContext = demoContext;
    _eventContexts = eventContexts;
  }

  public async Task Handle(InitializeDatabaseCommand _, CancellationToken cancellationToken)
  {
    foreach (EventContext eventContext in _eventContexts)
    {
      await eventContext.Database.MigrateAsync(cancellationToken);
    }

    await _demoContext.Database.MigrateAsync(cancellationToken);
  }
}
