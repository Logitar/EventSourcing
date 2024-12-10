namespace Logitar.EventSourcing;

public readonly struct StreamExpectation
{
  public enum StreamExpectationKind
  {
    None = 0,
    ShouldExist = 1,
    ShouldNotExist = 2,
    ShouldBeAtVersion = 3
  }

  public static StreamExpectation None => new();
  public static StreamExpectation ShouldExist => new(StreamExpectationKind.ShouldExist);
  public static StreamExpectation ShouldNotExist => new(StreamExpectationKind.ShouldNotExist);
  public static StreamExpectation ShouldBeAtVersion(long version) => new(version);

  public StreamExpectationKind Kind { get; }
  public long Version { get; }

  public StreamExpectation()
  {
  }

  public StreamExpectation(StreamExpectationKind kind)
  {
    if (kind == StreamExpectationKind.ShouldBeAtVersion)
    {
      throw new ArgumentOutOfRangeException(nameof(kind), "When expecting the stream to be at a specific version, use the constructor specifying a version.");
    }

    Kind = kind;
  }

  public StreamExpectation(long version)
  {
    if (version < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(version), "The version should be greater than 0.");
    }

    Kind = StreamExpectationKind.ShouldBeAtVersion;
    Version = version;
  }

  public static bool operator ==(StreamExpectation left, StreamExpectation right) => left.Equals(right);
  public static bool operator !=(StreamExpectation left, StreamExpectation right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is StreamExpectation expectation && expectation.Kind == Kind && expectation.Version == Version;
  public override int GetHashCode() => HashCode.Combine(Kind, Version);
  public override string ToString() => Kind == StreamExpectationKind.ShouldBeAtVersion ? $"{Kind}: {Version}" : Kind.ToString();
}
