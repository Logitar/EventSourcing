namespace Logitar.EventSourcing;

/// <summary>
/// Provides extension methods for the <see cref="List{T}"/> class.
/// </summary>
public static class ListExtensions
{
  /// <summary>
  /// Adds the specified item to the specified list, only if its not null. Otherwise, nothing happens.
  /// </summary>
  /// <typeparam name="T">The type of the list items.</typeparam>
  /// <param name="list">The list.</param>
  /// <param name="item">The item.</param>
  public static void AddIfNotNull<T>(this List<T> list, T? item)
  {
    if (item != null)
    {
      list.Add(item);
    }
  }
}
