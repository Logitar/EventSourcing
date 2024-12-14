using Logitar.EventSourcing.Demo.Infrastructure.Converters;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.Demo.Infrastructure;

internal class DemoEventSerializer : EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new CartIdConverter());
    SerializerOptions.Converters.Add(new ProductIdConverter());
  }
}
