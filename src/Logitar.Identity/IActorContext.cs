using Logitar.EventSourcing;

namespace Logitar.Identity;

/// <summary>
/// TODO(fpion): implement
/// </summary>
public interface IActorContext
{
  /// <summary>
  /// Gets the current actor identifier.
  /// </summary>
  AggregateId ActorId { get; }
}
