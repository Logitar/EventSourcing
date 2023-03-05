namespace Logitar.EventSourcing;

/// <summary>
/// Provides extension methods for the <see cref="List{T}"/> class.
/// </summary>
public static class ListExtensions
{
  /// <summary>
  /// Adds the specified item to the specified list, only if its not null. Otherwise, nothing happens.
  /// Returns a value indicating whether or not the specified item was added to the specified list.
  /// </summary>
  /// <typeparam name="T">The type of the list items.</typeparam>
  /// <param name="list">The list.</param>
  /// <param name="item">The item.</param>
  /// <returns>True if the item was added, false otherwise (if item was null).</returns>
  public static bool AddIfNotNull<T>(this List<T> list, T? item)
  {
    if (item == null)
    {
      return false;
    }

    list.Add(item);

    return true;
  }
}
