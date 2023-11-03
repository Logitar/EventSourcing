namespace Logitar.EventSourcing.Infrastructure;

internal record DefaultLanguageChangedEvent(CultureInfo Culture) : DomainEvent;
