using Bogus;
using Logitar.EventSourcing.Infrastructure;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

[Trait(Traits.Category, Categories.Unit)]
public class StreamEntityTests
{
  private readonly Faker _faker = new();
  private readonly EventSerializer _serializer = new();

  [Theory(DisplayName = "Append: it should throw StreamMismatchException when the event stream did not match the current stream.")]
  [InlineData(null)]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_StreamMismatch_When_Append_Then_StreamMismatchException(bool? isSameEntity)
  {
    StreamEntity stream = new(StreamId.NewId(), typeof(User));

    Type type = typeof(UserEnabled);
    EventEntity @event = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}");
    switch (isSameEntity)
    {
      case true:
        long streamId = @event.StreamId + 1;
        @event.GetType().GetProperty("StreamId")?.SetValue(@event, streamId);
        Assert.Equal(streamId, @event.StreamId);
        break;
      case false:
        StreamEntity otherStream = new(StreamId.NewId(), typeof(User));
        @event.GetType().GetProperty("Stream")?.SetValue(@event, otherStream);
        Assert.Same(otherStream, @event.Stream);
        break;
      default:
        @event.GetType().GetProperty("Stream")?.SetValue(@event, null);
        Assert.Null(@event.Stream);
        break;
    }

    var exception = Assert.Throws<StreamMismatchException>(() => stream.Append(@event));
    Assert.Equal(stream.Id, exception.StreamId);
    Assert.Equal(@event.Stream?.Id, exception.EventStreamId);
    Assert.Equal(@event.Id, exception.EventId);
  }

  [Fact(DisplayName = "Append: it should throw UnexpectedEventVersionException when the event version was not subsequent.")]
  public void Given_EventNotSubsequent_When_Append_Then_UnexpectedEventVersionException()
  {
    StreamEntity stream = new(StreamId.NewId());

    Type type = typeof(UserEnabled);
    EventEntity @event = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}");

    long version = @event.Version + 10;
    @event.GetType().GetProperty("Version")?.SetValue(@event, version);
    Assert.Equal(version, @event.Version);

    var exception = Assert.Throws<UnexpectedEventVersionException>(() => stream.Append(@event));
    Assert.Equal(stream.Id, exception.StreamId);
    Assert.Equal(stream.Version, exception.StreamVersion);
    Assert.Equal(@event.Id, exception.EventId);
    Assert.Equal(@event.Version, exception.EventVersion);
  }

  [Theory(DisplayName = "Append: it should update stream creation metadata when appending the first event.")]
  [InlineData(null, null)]
  [InlineData(null, false)]
  [InlineData("SYSTEM", true)]
  public void Given_FirstEventAppended_When_Append_Then_StreamCreated(string? actorIdValue, bool? isDeleted)
  {
    StreamEntity stream = new(StreamId.NewId(), typeof(User));

    Type type = typeof(UserEnabled);
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);
    EventEntity @event = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}", actorId, isDeleted);

    stream.Append(@event);

    Assert.Equal(@event.Version, stream.Version);
    Assert.Equal(actorIdValue, stream.CreatedBy);
    Assert.Equal(@event.OccurredOn, stream.CreatedOn);
    Assert.Equal(actorIdValue, stream.UpdatedBy);
    Assert.Equal(@event.OccurredOn, stream.UpdatedOn);
    Assert.Equal(isDeleted ?? false, stream.IsDeleted);
    Assert.Equal(@event, Assert.Single(stream.Events));
  }

  [Theory(DisplayName = "Append: it should update stream metadata when appending subsequent events.")]
  [InlineData(null, null)]
  [InlineData(null, false)]
  [InlineData("SYSTEM", true)]
  public void Given_SubsequentEventAppended_When_Append_Then_StreamUpdated(string? actorIdValue, bool? isDeleted)
  {
    StreamEntity stream = new(StreamId.NewId(), typeof(User));

    UserCreated created = new(_faker.Person.UserName)
    {
      StreamId = new StreamId(stream.Id),
      Version = 1,
      OccurredOn = DateTime.Now.AddHours(-1)
    };
    EventEntity event1 = new(created.Id, stream, created.OccurredOn, created.GetType().Name, created.GetType().GetNamespaceQualifiedName(), _serializer.Serialize(created));
    stream.Append(event1);

    Type type = typeof(UserEnabled);
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);
    EventEntity event2 = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}", actorId, isDeleted);

    stream.Append(event2);

    Assert.Equal(event2.Version, stream.Version);
    Assert.Equal(event1.ActorId, stream.CreatedBy);
    Assert.Equal(event1.OccurredOn, stream.CreatedOn);
    Assert.Equal(event2.ActorId, stream.UpdatedBy);
    Assert.Equal(event2.OccurredOn, stream.UpdatedOn);
    Assert.Equal(isDeleted ?? false, stream.IsDeleted);

    Assert.Equal(2, stream.Events.Count);
    Assert.Equal(event1, stream.Events.ElementAt(0));
    Assert.Equal(event2, stream.Events.ElementAt(1));
  }

  [Theory(DisplayName = "ctor: it should construct the correct instance of StreamENtity.")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_Arguments_When_ctor_Then_CorrectEntity(bool isTyped)
  {
    StreamId id = StreamId.NewId();
    Type? type = isTyped ? typeof(User) : null;

    StreamEntity entity = new(id, type);

    Assert.Equal(0, entity.StreamId);
    Assert.Equal(id.Value, entity.Id);
    Assert.Equal(type?.GetNamespaceQualifiedName(), entity.Type);
    Assert.Equal(0, entity.Version);
    Assert.Null(entity.CreatedBy);
    Assert.Equal(default, entity.CreatedOn);
    Assert.Null(entity.UpdatedBy);
    Assert.Equal(default, entity.UpdatedOn);
    Assert.False(entity.IsDeleted);
    Assert.Empty(entity.Events);
  }

  [Fact(DisplayName = "Equals: it should return false comparing objects of different types.")]
  public void Given_DifferentTypes_When_Equals_Then_False()
  {
    Type type = typeof(UserEnabled);

    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    EventEntity @event = new(EventId.NewId(), stream, DateTime.Now, type.Name, type.GetNamespaceQualifiedName(), "{}");
    Assert.False(stream.Equals(@event));
  }

  [Fact(DisplayName = "Equals: it should return false when the compared streams are different.")]
  public void Given_DifferentStreams_When_Equals_Then_False()
  {
    StreamEntity stream1 = new(StreamId.NewId(), typeof(User));
    StreamEntity stream2 = new(StreamId.NewId(), typeof(User));
    Assert.False(stream1.Equals(stream2));
  }

  [Fact(DisplayName = "Equals: it should return true when the compared streams are the same.")]
  public void Given_SameStreams_When_Equals_Then_True()
  {
    StreamEntity stream1 = new(StreamId.NewId(), typeof(User));
    StreamEntity stream2 = new(new StreamId(stream1.Id), stream1.GetStreamType());
    Assert.True(stream1.Equals(stream1));
    Assert.True(stream1.Equals(stream2));
  }

  [Fact(DisplayName = "GetHashCode: it should return the correct hash code.")]
  public void Given_EventEntity_When_GetHashCode_Then_CorrectHashCode()
  {
    StreamEntity stream = new(StreamId.NewId(), typeof(User));
    Assert.Equal(stream.Id.GetHashCode(), stream.GetHashCode());
  }

  [Fact(DisplayName = "GetStreamType: it should return null when the stream is not typed.")]
  public void Given_NoType_When_GetStreamType_Then_Null()
  {
    StreamEntity stream = new(StreamId.NewId());
    Assert.Null(stream.GetStreamType());
  }

  [Fact(DisplayName = "GetStreamType: it should return the resolved stream type.")]
  public void Given_StreamTypeResolved_When_GetStreamType_Then_TypeReturned()
  {
    Type type = typeof(User);
    StreamEntity stream = new(StreamId.NewId(), type);
    Assert.Equal(type, stream.GetStreamType());
  }

  [Fact(DisplayName = "GetStreamType: it should throw StreamTypeNotFoundException when the type was not resolved.")]
  public void Given_StreamTypeNotResolved_When_GetStreamType_Then_StreamTypeNotFoundException()
  {
    StreamEntity stream = new(StreamId.NewId());

    string type = "invalid_type";
    stream.GetType().GetProperty("Type")?.SetValue(stream, type);
    Assert.Equal(type, stream.Type);

    var exception = Assert.Throws<StreamTypeNotFoundException>(stream.GetStreamType);
    Assert.Equal(stream.Id, exception.StreamId);
    Assert.Equal(stream.Type, exception.TypeName);
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation.")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_EventEntity_When_ToString_Then_CorrectStringRepresentation(bool isTyped)
  {
    Type? type = isTyped ? typeof(User) : null;
    StreamEntity stream = new(StreamId.NewId(), type);

    if (isTyped)
    {
      Assert.Equal(
        string.Format("Logitar.EventSourcing.User, Logitar.EventSourcing.UnitTests | Logitar.EventSourcing.EntityFrameworkCore.Relational.StreamEntity (Id={0})", stream.Id),
        stream.ToString());
    }
    else
    {
      Assert.Equal(string.Format("Logitar.EventSourcing.EntityFrameworkCore.Relational.StreamEntity (Id={0})", stream.Id), stream.ToString());
    }
  }
}
