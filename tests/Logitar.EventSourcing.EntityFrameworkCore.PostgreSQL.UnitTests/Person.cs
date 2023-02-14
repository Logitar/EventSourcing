namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

internal record Person(string FullName)
{
  public List<Car> Cars { get; private set; } = new();
  public List<Pet> Pets { get; private set; } = new();
}
