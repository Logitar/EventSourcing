using Logitar.EventSourcing;

namespace Logitar.Identity;

/// <summary>
/// Represents the acting context of the identity system.
/// </summary>
public interface IActorContext
{
  /// <summary>
  /// Gets the current actor identifier.
  /// </summary>
  AggregateId ActorId { get; }
}
