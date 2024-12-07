namespace Logitar.EventSourcing.Infrastructure;

internal class DefaultLanguageChangedEvent : DomainEvent
{
  public CultureInfo Culture { get; }

  public DefaultLanguageChangedEvent(CultureInfo culture)
  {
    Culture = culture;
  }
}
