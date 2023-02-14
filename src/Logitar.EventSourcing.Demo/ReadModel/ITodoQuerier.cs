using Logitar.EventSourcing.Demo.Models;
using Logitar.EventSourcing.Demo.Models.Todos;

namespace Logitar.EventSourcing.Demo.ReadModel;

public interface ITodoQuerier
{
  Task<Todo?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PagedList<Todo>> GetPagedAsync(bool? isCompleted = null, string? search = null,
    TodoSort? sort = null, bool isDescending = false, int? skip = null, int? take = null, CancellationToken cancellationToken = default);
}
