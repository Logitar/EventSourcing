using Bogus;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

[Trait(Traits.Category, Categories.Unit)]
public class EventEntityTests
{
  private readonly Faker _faker = new();

  [Theory(DisplayName = "ctor: it should construct the correct instance of EventEntity.")]
  [InlineData(null, null)]
  [InlineData(null, false)]
  [InlineData("SYSTEM", true)]
  public void Given_Arguments_When_ctor_Then_CorrectEntity(string? actorIdValue, bool? isDeleted)
  {
    EventId id = EventId.NewId();
    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    DateTime occurredOn = DateTime.Now.AddDays(-1);
    string typeName = typeof(UserEnabled).Name;
    string namespacedType = typeof(UserEnabled).GetNamespaceQualifiedName();
    string data = "{}";
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    EventEntity entity = new(id, stream, occurredOn, typeName, namespacedType, data, actorId, isDeleted);

    Assert.Equal(0, entity.EventId);
    Assert.Equal(id.Value, entity.Id);
    Assert.Same(stream, entity.Stream);
    Assert.Equal(stream.StreamId, entity.StreamId);
    Assert.Equal(stream.Version + 1, entity.Version);
    Assert.Equal(actorIdValue, entity.ActorId);
    Assert.Equal(occurredOn.AsUniversalTime(), entity.OccurredOn);
    Assert.Equal(isDeleted, entity.IsDeleted);
    Assert.Equal(typeName, entity.TypeName);
    Assert.Equal(namespacedType, entity.NamespacedType);
    Assert.Equal(data, entity.Data);
  }

  [Fact(DisplayName = "Equals: it should return false comparing objects of different types.")]
  public void Given_DifferentTypes_When_Equals_Then_False()
  {
    Type type = typeof(UserEnabled);

    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    EventEntity event1 = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}");
    UserCreated event2 = new(_faker.Person.UserName);
    Assert.False(event1.Equals(event2));
  }

  [Fact(DisplayName = "Equals: it should return false when the compared events are different.")]
  public void Given_DifferentEvents_When_Equals_Then_False()
  {
    Type type = typeof(UserEnabled);

    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    EventEntity event1 = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}");
    EventEntity event2 = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}");
    Assert.False(event1.Equals(event2));
  }

  [Fact(DisplayName = "Equals: it should return true when the compared events are the same.")]
  public void Given_SameEvents_When_Equals_Then_True()
  {
    Type type = typeof(UserEnabled);

    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    EventEntity event1 = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}");
    EventEntity event2 = new(new EventId(event1.Id), stream, event1.OccurredOn, event1.TypeName, event1.NamespacedType, event1.Data);
    Assert.True(event1.Equals(event1));
    Assert.True(event1.Equals(event2));
  }

  [Fact(DisplayName = "GetDataType: it should return the correct data type.")]
  public void Given_DataTypeResolved_When_GetDataType_Then_CorrectType()
  {
    Type type = typeof(UserEnabled);

    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    EventEntity entity = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}");

    Assert.Equal(type, entity.GetDataType());
  }

  [Fact(DisplayName = "GetDataType: it should throw EventTypeNotFoundException when the data type could not be resolved.")]
  public void Given_DataTypeNotResolved_When_GetDataType_Then_EventTypeNotFoundException()
  {
    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    EventEntity entity = new(EventId.NewId(), stream, DateTime.Now, typeof(UserEnabled).Name, "invalid_type", "{}");

    var exception = Assert.Throws<EventTypeNotFoundException>(entity.GetDataType);
    Assert.Equal(entity.Id, exception.EventId);
    Assert.Equal(entity.NamespacedType, exception.TypeName);
  }

  [Fact(DisplayName = "GetHashCode: it should return the correct hash code.")]
  public void Given_EventEntity_When_GetHashCode_Then_CorrectHashCode()
  {
    Type type = typeof(UserEnabled);

    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    EventEntity entity = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}");

    Assert.Equal(entity.Id.GetHashCode(), entity.GetHashCode());
  }

  [Fact(DisplayName = "ToString: it should return the correct string representation.")]
  public void Given_EventEntity_When_ToString_Then_CorrectStringRepresentation()
  {
    Type type = typeof(UserEnabled);

    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    EventEntity entity = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}");

    Assert.Equal(string.Format("UserEnabled | Logitar.EventSourcing.EntityFrameworkCore.Relational.EventEntity (Id={0})", entity.Id), entity.ToString());
  }
}
