namespace Logitar.EventSourcing.Demo.Domain;

internal class TodoAggregate : AggregateRoot
{
  public TodoAggregate(AggregateId id) : base(id)
  {
  }
  public TodoAggregate(AggregateId actorId, string name) : base()
  {
    ApplyChange(new TodoCreated(ValidateName(name))
    {
      ActorId = actorId
    });
  }

  public string Name { get; private set; } = string.Empty;
  public bool IsCompleted { get; private set; }

  protected virtual void Apply(TodoCreated e)
  {
    Name = e.Name;
  }

  public void Delete(AggregateId actorId)
  {
    ApplyChange(new TodoDeleted
    {
      ActorId = actorId
    });
  }
  protected virtual void Apply(TodoDeleted e)
  {
  }

  public void Rename(AggregateId actorId, string newName)
  {
    ApplyChange(new TodoRenamed(ValidateName(newName, nameof(newName)))
    {
      ActorId = actorId
    });
  }
  protected virtual void Apply(TodoRenamed e)
  {
    Name = e.NewName;
  }

  public void Toggle(AggregateId actorId)
  {
    ApplyChange(new TodoToggled(!IsCompleted)
    {
      ActorId = actorId
    });
  }
  protected virtual void Apply(TodoToggled e)
  {
    IsCompleted = e.IsCompleted;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";

  private static string ValidateName(string name, string? paramName = null)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("The Todo name is required.", paramName);
    }

    name = name.Trim();
    if (name.Length > byte.MaxValue)
    {
      throw new ArgumentException($"The Todo name cannot exceed {byte.MaxValue} characters.", paramName);
    }

    return name;
  }
}
