namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;

public class DeleteActionEntity : EnumEntity<int>
{
  private DeleteActionEntity() : base()
  {
  }
  private DeleteActionEntity(DeleteAction value) : base((int)value, value.ToString())
  {
  }

  public static IEnumerable<DeleteActionEntity> GetData() => Enum.GetValues<DeleteAction>().Select(value => new DeleteActionEntity(value));
}
