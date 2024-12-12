namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

public class SystemStarted : ITemporalEvent
{
  public DateTime OccurredOn { get; set; } = DateTime.Now;
}
