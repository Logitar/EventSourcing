namespace Logitar.EventSourcing.Demo.Models.Todos;

public record RenameTodoInput
{
  public string NewName { get; init; } = string.Empty;
}
