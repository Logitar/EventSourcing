namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model representing a delete action.
/// </summary>
public class DeleteActionEntity : EnumEntity<int>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DeleteActionEntity"/> class.
  /// </summary>
  private DeleteActionEntity() : base()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="DeleteActionEntity"/> class using a value of the <see cref="DeleteAction"/> enumeration.
  /// </summary>
  /// <param name="value">The enumeration value</param>
  private DeleteActionEntity(DeleteAction value) : base((int)value, value.ToString())
  {
  }

  /// <summary>
  /// Gets the seed data from the values of the <see cref="DeleteAction"/> enumeration.
  /// </summary>
  /// <returns>The list of entities</returns>
  public static IEnumerable<DeleteActionEntity> GetData() => Enum.GetValues<DeleteAction>().Select(value => new DeleteActionEntity(value));
}
