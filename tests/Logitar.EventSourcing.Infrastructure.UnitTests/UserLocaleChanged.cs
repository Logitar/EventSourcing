namespace Logitar.EventSourcing.Infrastructure;

public record UserLocaleChanged(CultureInfo Locale) : IEvent;
