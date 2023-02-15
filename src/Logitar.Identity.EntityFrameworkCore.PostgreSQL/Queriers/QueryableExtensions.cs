namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Queriers;

/// <summary>
/// Provides extension methods for the <see cref="IQueryable{T}"/> class.
/// </summary>
public static class QueryableExtensions
{
  /// <summary>
  /// Applies paging parameters to the specified query.
  /// </summary>
  /// <typeparam name="T">The type of the query items.</typeparam>
  /// <param name="query">The query to apply paging to.</param>
  /// <param name="skip">The number of items to skip.</param>
  /// <param name="take">The number of items to fetch.</param>
  /// <returns>The parametrized query.</returns>
  public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int? skip, int? take)
  {
    if (skip.HasValue)
    {
      query = query.Skip(skip.Value);
    }

    if (take.HasValue)
    {
      query = query.Take(take.Value);
    }

    return query;
  }
}
