using Bogus;

namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class StreamTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "ctor: it should construct the stream from arguments.")]
  public void Given_Arguments_When_ctor_Then_StreamIsConstructed()
  {
    StreamId id = StreamId.NewId();
    Type type = typeof(User);

    List<Event> events =
    [
      new Event(EventId.NewId(), 1, DateTime.Now.AddDays(-1), new UserCreated(_faker.Person.UserName), new ActorId("SYSTEM")),
      new Event(EventId.NewId(), 2, DateTime.Now.AddHours(-1), new UserPasswordCreated(Convert.ToBase64String(Encoding.UTF8.GetBytes("P@s$W0rD")))),
      new Event(EventId.NewId(), 3, DateTime.Now, new UserDeleted(), new ActorId(id.Value), isDeleted: true)
    ];

    Stream stream = new(id, type, events);

    Assert.Equal(id, stream.Id);
    Assert.Equal(type, stream.Type);
    Assert.Equal(events.Last().Version, stream.Version);
    Assert.Equal(events.First().ActorId, stream.CreatedBy);
    Assert.Equal(events.First().OccurredOn, stream.CreatedOn);
    Assert.Equal(events.Last().ActorId, stream.UpdatedBy);
    Assert.Equal(events.Last().OccurredOn, stream.UpdatedOn);
    Assert.True(stream.IsDeleted);
    Assert.Equal(events, stream.Events);
  }

  [Fact(DisplayName = "ctor: it should construct the stream from default arguments.")]
  public void Given_DefaultArguments_When_ctor_Then_StreamIsConstructed()
  {
    StreamId id = StreamId.NewId();

    Stream stream = new(id);

    Assert.Equal(id, stream.Id);
    Assert.Null(stream.Type);
    Assert.Equal(0, stream.Version);
    Assert.Null(stream.CreatedBy);
    Assert.Null(stream.CreatedOn);
    Assert.Null(stream.CreatedBy);
    Assert.Null(stream.UpdatedOn);
    Assert.False(stream.IsDeleted);
    Assert.Empty(stream.Events);
  }

  [Fact(DisplayName = "Equals: it should return false when comparing different streams.")]
  public void Given_DifferentStreams_When_Equals_Then_False()
  {
    Stream stream1 = new(StreamId.NewId());
    Stream stream2 = new(StreamId.NewId());
    Assert.False(stream1.Equals(stream2));
  }

  [Fact(DisplayName = "Equals: it should return false when comparing with an object of a different type.")]
  public void Given_DifferentTypes_When_Equals_Then_False()
  {
    Stream stream = new(StreamId.NewId());
    User user = new();
    Assert.False(stream.Equals(user));
  }

  [Fact(DisplayName = "Equals: it should return true when comparing the same stream.")]
  public void Given_SameStream_When_Equals_Then_True()
  {
    Stream stream1 = new(StreamId.NewId());
    Stream stream2 = new(stream1.Id);
    Assert.True(stream1.Equals(stream1));
    Assert.True(stream1.Equals(stream2));
  }

  [Fact(DisplayName = "GetHashCode: it should return the correct hash code.")]
  public void Given_Stream_When_GetHashCode_Then_CorrectHashCode()
  {
    Stream stream = new(StreamId.NewId());
    Assert.Equal(stream.Id.GetHashCode(), stream.GetHashCode());
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation.")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_Stream_When_ToString_Then_CorrectStringRepresentation(bool typed)
  {
    Type? type = typed ? typeof(User) : null;
    Stream stream = new(StreamId.NewId(), type);
    Assert.Equal(typed
      ? string.Format("Logitar.EventSourcing.User | Logitar.EventSourcing.Stream (Id={0})", stream.Id)
      : string.Format("Logitar.EventSourcing.Stream (Id={0})", stream.Id), stream.ToString());
  }
}
