namespace Logitar.EventSourcing;

internal class Session : AggregateRoot
{
  public Session() : base()
  {
  }

  public Session(StreamId? id) : base(id)
  {
  }
}
