using Logitar.EventSourcing.Demo.Domain;
using Logitar.EventSourcing.Demo.Models;
using Logitar.EventSourcing.Demo.Models.Todos;
using Logitar.EventSourcing.Demo.ReadModel;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.EventSourcing.Demo.Controllers;

[ApiController]
[Route("todos")]
public class TodoController : ControllerBase
{
  private readonly IEventStore _eventStore;
  private readonly ITodoQuerier _todoQuerier;

  public TodoController(IEventStore eventStore, ITodoQuerier todoQuerier)
  {
    _eventStore = eventStore;
    _todoQuerier = todoQuerier;
  }

  protected AggregateId ActorId => new(Request.Headers[Constants.Headers.XUser].SingleOrDefault() ?? "SYSTEM");

  [HttpPost]
  public async Task<ActionResult<Todo>> CreateAsync([FromBody] CreateTodoInput input, CancellationToken cancellationToken)
  {
    TodoAggregate todo = new(ActorId, input.Name);
    await _eventStore.SaveAsync(todo, cancellationToken);

    Todo output = new(todo);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/todos/{output.Id}", UriKind.Absolute);

    return Created(uri, output);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<Todo>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    TodoAggregate? todo = await _eventStore.LoadAsync<TodoAggregate>(new AggregateId(id), cancellationToken: cancellationToken);
    if (todo == null)
    {
      return NotFound();
    }

    todo.Delete(ActorId);
    await _eventStore.SaveAsync(todo, cancellationToken);

    return Ok(new Todo(todo));
  }

  [HttpGet]
  public async Task<ActionResult<PagedList<Todo>>> GetAsync(bool? isCompleted, string? search,
    TodoSort? sort, bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    return Ok(await _todoQuerier.GetPagedAsync(isCompleted, search, sort, isDescending, skip, take, cancellationToken));
  }


  [HttpGet("{id}")]
  public async Task<ActionResult<Todo>> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    Todo? todo = await _todoQuerier.GetAsync(id, cancellationToken);
    if (todo == null)
    {
      return NotFound();
    }

    return Ok(todo);
  }

  [HttpPatch("{id}/rename")]
  public async Task<ActionResult<Todo>> RenameAsync(Guid id, [FromBody] RenameTodoInput input, CancellationToken cancellationToken)
  {
    TodoAggregate? todo = await _eventStore.LoadAsync<TodoAggregate>(new AggregateId(id), cancellationToken: cancellationToken);
    if (todo == null)
    {
      return NotFound();
    }

    todo.Rename(ActorId, input.NewName);
    await _eventStore.SaveAsync(todo, cancellationToken);

    return Ok(new Todo(todo));
  }

  [HttpPatch("{id}/toggle")]
  public async Task<ActionResult<Todo>> ToggleAsync(Guid id, CancellationToken cancellationToken)
  {
    TodoAggregate? todo = await _eventStore.LoadAsync<TodoAggregate>(new AggregateId(id), cancellationToken: cancellationToken);
    if (todo == null)
    {
      return NotFound();
    }

    todo.Toggle(ActorId);
    await _eventStore.SaveAsync(todo, cancellationToken);

    return Ok(new Todo(todo));
  }
}
