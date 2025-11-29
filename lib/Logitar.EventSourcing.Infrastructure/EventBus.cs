using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.EventSourcing.Infrastructure;

/// <summary>
/// Implements an in-memory event bus into which published events are handled synchronously.
/// </summary>
public class EventBus : IEventBus
{
  /// <summary>
  /// Gets the service provider.
  /// </summary>
  protected IServiceProvider ServiceProvider { get; }

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
  public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
  {
    IEnumerable<object?> handlers = ServiceProvider.GetServices(typeof(IEventHandler<>).MakeGenericType(@event.GetType()));
    if (handlers.Any())
    {
      Type[] parameterTypes = [@event.GetType(), typeof(CancellationToken)];
      object[] parameters = [@event, cancellationToken];
      foreach (object? handler in handlers)
      {
        if (handler is not null)
        {
          MethodInfo? handle = handler.GetType().GetMethod(nameof(IEventHandler<>.HandleAsync), parameterTypes);
          if (handle is not null)
          {
            await (Task)handle.Invoke(handler, parameters)!;
          }
        }
      }
    }
  }
}
