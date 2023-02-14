namespace Logitar.EventSourcing.Demo.Models.Todos;

public record CreateTodoInput
{
  public string Name { get; init; } = string.Empty;
}
