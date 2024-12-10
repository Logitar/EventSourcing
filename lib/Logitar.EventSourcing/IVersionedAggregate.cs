namespace Logitar.EventSourcing;

public interface IVersionedAggregate : IAggregate
{
  long Version { get; }
}
