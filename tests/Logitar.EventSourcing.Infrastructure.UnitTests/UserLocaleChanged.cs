using System.Globalization;

namespace Logitar.EventSourcing.Infrastructure;

internal record UserLocaleChanged(CultureInfo Locale) : IEvent;
