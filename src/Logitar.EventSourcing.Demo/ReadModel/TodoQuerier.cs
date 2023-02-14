using Logitar.EventSourcing.Demo.Models;
using Logitar.EventSourcing.Demo.Models.Todos;
using Logitar.EventSourcing.Demo.ReadModel.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.EventSourcing.Demo.ReadModel;

internal class TodoQuerier : ITodoQuerier
{
  private readonly DbSet<TodoEntity> _todos;

  public TodoQuerier(TodoContext context)
  {
    _todos = context.Todos;
  }

  public async Task<Todo?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    TodoEntity? todo = await _todos.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(id).Value, cancellationToken);

    return todo == null ? null : new Todo(todo);
  }

  public async Task<PagedList<Todo>> GetPagedAsync(bool? isCompleted, string? search,
    TodoSort? sort, bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    IQueryable<TodoEntity> query = _todos.AsNoTracking();

    if (isCompleted.HasValue)
    {
      query = query.Where(x => x.IsCompleted == isCompleted.Value);
    }
    if (search != null)
    {
      query = query.Where(x => EF.Functions.ILike(x.Name, $"{search.Trim()}%"));
    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      switch (sort.Value)
      {
        case TodoSort.Name:
          query = isDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
          break;
        case TodoSort.UpdatedOn:
          query = isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn);
          break;
      }
    }

    if (skip.HasValue)
    {
      query = query.Skip(skip.Value);
    }
    if (take.HasValue)
    {
      query = query.Take(take.Value);
    }

    TodoEntity[] todos = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Todo>(todos.Select(todo => new Todo(todo)), total);
  }
}
