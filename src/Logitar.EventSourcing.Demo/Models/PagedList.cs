namespace Logitar.EventSourcing.Demo.Models;

public record PagedList<T>
{
  public PagedList() : this(Enumerable.Empty<T>())
  {
  }
  public PagedList(IEnumerable<T> items) : this(items, items.LongCount())
  {
  }
  public PagedList(long total) : this(Enumerable.Empty<T>(), total)
  {
  }
  public PagedList(IEnumerable<T> items, long total)
  {
    Items = items;
    Total = total;
  }

  public IEnumerable<T> Items { get; init; }
  public long Total { get; init; }
}
