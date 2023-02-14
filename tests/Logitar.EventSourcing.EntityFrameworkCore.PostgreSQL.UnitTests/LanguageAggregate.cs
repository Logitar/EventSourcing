using System.Globalization;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

internal class LanguageAggregate : AggregateRoot
{
  public LanguageAggregate(CultureInfo culture) : base()
  {
    ApplyChange(new LanguageCreated(culture));
  }

  public CultureInfo Culture { get; private set; } = CultureInfo.InvariantCulture;

  protected virtual void Apply(LanguageCreated e)
  {
    Culture = e.Culture;
  }
}
