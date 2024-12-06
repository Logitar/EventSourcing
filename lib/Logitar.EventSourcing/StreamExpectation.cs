using System;

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

  public static StreamExpectation AtVersion(long version) => new(version);

  public StreamExpectationKind Kind { get; }
  public long Version { get; }

  public StreamExpectation()
  {
  }
  public StreamExpectation(StreamExpectationKind kind)
  {
    Kind = kind;
  }
  public StreamExpectation(long version)
  {
    Kind = StreamExpectationKind.ShouldBeAtVersion;
    Version = version;
  }

  public static bool operator ==(StreamExpectation left, StreamExpectation right) => left.Equals(right);
  public static bool operator !=(StreamExpectation left, StreamExpectation right) => !left.Equals(right);

  public override bool Equals(object obj) => obj is StreamExpectation expectation && expectation.Kind == Kind && expectation.Version == Version;
  public override int GetHashCode() => HashCode.Combine(Kind, Version);
  public override string ToString() => Kind == StreamExpectationKind.ShouldBeAtVersion ? $"{Kind}: {Version}" : Kind.ToString();
}
