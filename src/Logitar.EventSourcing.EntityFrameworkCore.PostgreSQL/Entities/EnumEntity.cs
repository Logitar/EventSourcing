namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;

public abstract class EnumEntity<T>
{
  protected EnumEntity()
  {
  }
  protected EnumEntity(T value, string name)
  {
    Value = value;
    Name = name;
  }

  public T Value { get; private set; } = default!;
  public string Name { get; private set; } = string.Empty;
}
