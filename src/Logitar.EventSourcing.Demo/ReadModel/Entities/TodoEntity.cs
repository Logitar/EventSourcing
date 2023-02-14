using Logitar.EventSourcing.Demo.Domain;

namespace Logitar.EventSourcing.Demo.ReadModel.Entities;

internal class TodoEntity : AggregateEntity
{
  public TodoEntity(TodoCreated e) : base(e)
  {
    Name = e.Name;
  }
  private TodoEntity() : base()
  {
  }

  public int TodoId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public bool IsCompleted { get; private set; }

  public void Rename(TodoRenamed e)
  {
    Update(e);

    Name = e.NewName;
  }
  public void Toggle(TodoToggled e)
  {
    Update(e);

    IsCompleted = e.IsCompleted;
  }
}
