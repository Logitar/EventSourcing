using Logitar.EventSourcing;

namespace Logitar.Identity;

/// <summary>
/// Represents the application context of the identity services.
/// </summary>
public interface IIdentityContext
{
  /// <summary>
  /// Gets the current actor identifier.
  /// </summary>
  AggregateId ActorId { get; }
}
