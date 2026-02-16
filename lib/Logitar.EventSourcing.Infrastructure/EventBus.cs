using Microsoft.Extensions.DependencyInjection;

namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Implements an in-memory event bus into which published events are handled synchronously.
/// </summary>
public class EventBus : IEventBus
{
  /// <summary>
  /// The name of the handler method.
  /// </summary>
  protected const string HandlerName = nameof(IEventHandler<>.HandleAsync);

  /// <summary>
  /// Gets the service provider.
  /// </summary>
  protected virtual IServiceProvider ServiceProvider { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="EventBus"/> class.
  /// </summary>
  /// <param name="serviceProvider">The service provider.</param>
  public EventBus(IServiceProvider serviceProvider)
  {
    ServiceProvider = serviceProvider;
  }

  /// <summary>
  /// Publishes the specified event to the event bus.
  /// </summary>
  /// <param name="event">The event to publish.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  public virtual async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<object> handlers = await GetHandlersAsync(@event, cancellationToken);
    if (handlers.Count > 0)
    {
      Type[] parameterTypes = [@event.GetType(), typeof(CancellationToken)];
      object[] parameters = [@event, cancellationToken];
      foreach (object handler in handlers)
      {
        Type handlerType = handler.GetType();
        MethodInfo handle = handlerType.GetMethod(HandlerName, parameterTypes)
          ?? throw new InvalidOperationException($"The handler {handlerType} must define a '{HandlerName}' method.");
        if (handle.Invoke(handler, parameters) is Task task)
        {
          await task;
        }
        else
        {
          throw new InvalidOperationException($"The handler {handlerType} {HandlerName} method must return a {nameof(Task)}.");
        }
      }
    }
  }

  /// <summary>
  /// Finds the handlers of the specified event.
  /// </summary>
  /// <param name="event">The event.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The event handlers.</returns>
  protected virtual async Task<IReadOnlyCollection<object>> GetHandlersAsync(IEvent @event, CancellationToken cancellationToken)
  {
    return ServiceProvider.GetServices(typeof(IEventHandler<>).MakeGenericType(@event.GetType()))
      .Where(handler => handler is not null)
      .Select(handler => handler!)
      .ToList()
      .AsReadOnly();
  }
}
