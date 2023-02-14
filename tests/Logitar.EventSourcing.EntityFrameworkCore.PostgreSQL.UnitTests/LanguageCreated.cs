using System.Globalization;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

internal record LanguageCreated(CultureInfo Culture) : DomainEvent;
