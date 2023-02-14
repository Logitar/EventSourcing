using Logitar.EventSourcing.Demo.Domain;
using Logitar.EventSourcing.Demo.ReadModel.Entities;

namespace Logitar.EventSourcing.Demo.Models.Todos;

public record Todo
{
  public Todo()
  {
  }

  internal Todo(TodoAggregate todo)
  {
    Id = todo.Id.ToGuid();
    Version = todo.Version;

    CreatedById = todo.CreatedById.Value;
    CreatedOn = todo.CreatedOn;

    UpdatedById = todo.CreatedById == todo.UpdatedById ? null : todo.UpdatedById.Value;
    UpdatedOn = todo.CreatedOn == todo.UpdatedOn ? null : todo.UpdatedOn;

    Name = todo.Name;
    IsCompleted = todo.IsCompleted;
  }
  internal Todo(TodoEntity todo)
  {
    Id = new AggregateId(todo.AggregateId).ToGuid();
    Version = todo.Version;

    CreatedById = todo.CreatedById;
    CreatedOn = todo.CreatedOn;

    UpdatedById = todo.UpdatedById;
    UpdatedOn = todo.UpdatedOn;

    Name = todo.Name;
    IsCompleted = todo.IsCompleted;
  }

  public Guid Id { get; init; }
  public long Version { get; init; }

  public string CreatedById { get; init; } = string.Empty;
  public DateTime CreatedOn { get; init; }

  public string? UpdatedById { get; init; }
  public DateTime? UpdatedOn { get; init; }

  public string Name { get; init; } = string.Empty;
  public bool IsCompleted { get; init; }
}
