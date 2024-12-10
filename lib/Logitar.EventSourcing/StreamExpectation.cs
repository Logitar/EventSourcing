namespace Logitar.EventSourcing;

/// <summary>
/// Represents an expectation of the state of an event expectation.
/// </summary>
public readonly struct StreamExpectation
{
  /// <summary>
  /// Represents the possible expectation kinds.
  /// </summary>
  public enum StreamExpectationKind
  {
    /// <summary>
    /// There is no expectation.
    /// </summary>
    None = 0,

    /// <summary>
    /// The expectation should exist before appending the current events.
    /// </summary>
    ShouldExist = 1,

    /// <summary>
    /// The expectation should not exist before appending the current events, and it will be created.
    /// </summary>
    ShouldNotExist = 2,

    /// <summary>
    /// The expectation should be at the specified version after appending the current events.
    /// </summary>
    ShouldBeAtVersion = 3
  }

  /// <summary>
  /// Returns no expectation expectation.
  /// </summary>
  public static StreamExpectation None => new();
  /// <summary>
  /// Returns an expectation of a expectation that exists before appending the current events.
  /// </summary>
  public static StreamExpectation ShouldExist => new(StreamExpectationKind.ShouldExist);
  /// <summary>
  /// Returns an expectation of a expectation that does not exist before appending the current events.
  /// </summary>
  public static StreamExpectation ShouldNotExist => new(StreamExpectationKind.ShouldNotExist);
  /// <summary>
  /// Returns an expectation of a expectation that is at the specified version after appending the current events.
  /// </summary>
  /// <param name="version">The expected expectation version after appending the current events.</param>
  /// <returns>The expectation expectation.</returns>
  public static StreamExpectation ShouldBeAtVersion(long version) => new(version);

  /// <summary>
  /// Gets the expectation kind.
  /// </summary>
  public StreamExpectationKind Kind { get; }
  /// <summary>
  /// Gets the expected version.
  /// </summary>
  public long Version { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamExpectation"/> struct.
  /// </summary>
  public StreamExpectation()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamExpectation"/> struct.
  /// </summary>
  /// <param name="kind">The expectation kind.</param>
  /// <exception cref="ArgumentOutOfRangeException">The expectation kind was <see cref="StreamExpectationKind.ShouldBeAtVersion"/>.</exception>
  public StreamExpectation(StreamExpectationKind kind)
  {
    if (kind == StreamExpectationKind.ShouldBeAtVersion)
    {
      throw new ArgumentOutOfRangeException(nameof(kind), "When expecting the expectation to be at a specific version, use the constructor specifying a version.");
    }

    Kind = kind;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamExpectation"/> struct.
  /// </summary>
  /// <param name="version">The expected expectation version.</param>
  /// <exception cref="ArgumentOutOfRangeException">The version was less than or equal to 0.</exception>
  public StreamExpectation(long version)
  {
    if (version < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(version), "The version should be greater than 0.");
    }

    Kind = StreamExpectationKind.ShouldBeAtVersion;
    Version = version;
  }

  /// <summary>
  /// Returns a value indicating whether or not the specified expectations are equal.
  /// </summary>
  /// <param name="left">The first expectation to compare.</param>
  /// <param name="right">The other expectation to compare.</param>
  /// <returns>True if the expectations are equal.</returns>
  public static bool operator ==(StreamExpectation left, StreamExpectation right) => left.Equals(right);
  /// <summary>
  /// Returns a value indicating whether or not the specified expectations are different.
  /// </summary>
  /// <param name="left">The first expectation to compare.</param>
  /// <param name="right">The other expectation to compare.</param>
  /// <returns>True if the expectations are different.</returns>
  public static bool operator !=(StreamExpectation left, StreamExpectation right) => !left.Equals(right);

  /// <summary>
  /// Returns a value indicating whether or not the specified object is equal to the expectation.
  /// </summary>
  /// <param name="obj">The object to be compared to.</param>
  /// <returns>True if the object is equal to the expectation.</returns>
  public override bool Equals([NotNullWhen(true)] object? obj) => obj is StreamExpectation expectation && expectation.Kind == Kind && expectation.Version == Version;
  /// <summary>
  /// Returns the hash code of the current expectation.
  /// </summary>
  /// <returns>The hash code.</returns>
  public override int GetHashCode() => HashCode.Combine(Kind, Version);
  /// <summary>
  /// Returns a string representation of the expectation.
  /// </summary>
  /// <returns>The string representation.</returns>
  public override string ToString() => Kind == StreamExpectationKind.ShouldBeAtVersion ? $"{Kind}: {Version}" : Kind.ToString();
}
