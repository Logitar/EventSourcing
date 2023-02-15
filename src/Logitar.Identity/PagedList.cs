namespace Logitar.Identity;

/// <summary>
/// Represents a paged list of items of the specified type.
/// </summary>
/// <typeparam name="T">The type of the items.</typeparam>
public record PagedList<T>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
  /// </summary>
  public PagedList() : this(Enumerable.Empty<T>())
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="PagedList{T}"/> class using the specified items.
  /// </summary>
  /// <param name="items">The list items.</param>
  public PagedList(IEnumerable<T> items) : this(items, items.LongCount())
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="PagedList{T}"/> class using the specified total count.
  /// </summary>
  /// <param name="total">The total count of items.</param>
  public PagedList(long total) : this(Enumerable.Empty<T>(), total)
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="PagedList{T}"/> class using the specified items and total count.
  /// </summary>
  /// <param name="items">The list items.</param>
  /// <param name="total">The total count of items.</param>
  public PagedList(IEnumerable<T> items, long total)
  {
    Items = items;
    Total = total;
  }

  /// <summary>
  /// Gets or sets the list items.
  /// </summary>
  public IEnumerable<T> Items { get; init; }
  /// <summary>
  /// Gets or sets the list total count.
  /// </summary>
  public long Total { get; init; }
}
