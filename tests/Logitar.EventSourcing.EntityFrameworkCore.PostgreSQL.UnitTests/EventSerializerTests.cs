using System.Globalization;
using System.Text.Json;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;

[Trait(Traits.Category, Categories.Unit)]
public class EventSerializerTests
{
  [Fact]
  public void Given_converter_is_registered_Then_it_is_used()
  {
    EventSerializer.Instance.RegisterConverter(new CarConverter());

    Person person = new("Martin Plante");

    Car car = new(2001, "Honda", "Civic", person);
    person.Cars.Add(car);

    Pet pet = new("Dog", "Brutus", person);
    person.Pets.Add(pet);

    PetRegistered e1 = new(pet);
    Assert.Throws<JsonException>(() => EventSerializer.Instance.Serialize(e1));

    CarRegistered e2 = new(car);
    string json = EventSerializer.Instance.Serialize(e2);
    Assert.Contains(@"""Car"":""2001|Honda|Civic|Martin Plante""", json);
  }

  [Fact]
  public void Given_domain_event_Then_it_is_serialized()
  {
    LanguageCreated e = new(CultureInfo.GetCultureInfo("en-CA"))
    {
      Id = Guid.Parse("4c064c15-240a-4cf3-a102-7b8ff3716449"),
      AggregateId = new AggregateId("4JGvMRWGlUuhu1AJhK8IQQ"),
      Version = 1,
      OccurredOn = DateTime.Parse("2023-02-13T14:49:54.6681361Z").ToUniversalTime()
    };
    string json = EventSerializer.Instance.Serialize(e);
    Assert.Equal("{\"Culture\":\"en-CA\",\"Id\":\"4c064c15-240a-4cf3-a102-7b8ff3716449\",\"AggregateId\":\"4JGvMRWGlUuhu1AJhK8IQQ\",\"Version\":1,\"ActorId\":\"SYSTEM\",\"OccurredOn\":\"2023-02-13T14:49:54.6681361Z\",\"DeleteAction\":0}", json);
  }

  [Fact]
  public void Given_JSON_Then_it_is_deserialized()
  {
    LanguageAggregate aggregate = new(CultureInfo.GetCultureInfo("en-CA"));
    LanguageCreated expected = (LanguageCreated?)aggregate.Changes.Single()!;
    Assert.NotNull(expected);

    EventEntity entity = EventEntity.FromChanges(aggregate).Single();
    LanguageCreated actual = (LanguageCreated?)EventSerializer.Instance.Deserialize(entity)!;
    Assert.NotNull(actual);

    Assert.Equal(expected, actual);
  }

  [Fact]
  public void Given_singleton_is_constructed_Then_it_is_the_same_instance()
  {
    EventSerializer e1 = EventSerializer.Instance;
    EventSerializer e2 = EventSerializer.Instance;
    Assert.Same(e1, e2);
  }
}
