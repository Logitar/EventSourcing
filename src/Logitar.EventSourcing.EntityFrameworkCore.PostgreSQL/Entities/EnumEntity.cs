namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;

/// <summary>
/// The database model used to represent an enumeration value.
/// </summary>
/// <typeparam name="T">The type of the enumeration value</typeparam>
public abstract class EnumEntity<T>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EnumEntity{T}"/> class.
  /// </summary>
  protected EnumEntity()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="EnumEntity{T}"/> class using the specified value and name.
  /// </summary>
  /// <param name="value">The enumeration value</param>
  /// <param name="name">The enumeration name</param>
  protected EnumEntity(T value, string name)
  {
    Value = value;
    Name = name;
  }

  /// <summary>
  /// The enumeration unique value
  /// </summary>
  public T Value { get; private set; } = default!;
  /// <summary>
  /// The enumeration unique name
  /// </summary>
  public string Name { get; private set; } = string.Empty;
}
