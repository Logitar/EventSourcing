namespace Logitar.EventSourcing.Demo.Application;

public interface IApplicationContext
{
  ActorId? ActorId { get; }
}
