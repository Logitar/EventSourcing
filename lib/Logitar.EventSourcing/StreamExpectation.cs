namespace Logitar.EventSourcing;

/// <summary>
/// Defines an expectation of the state of a stream of events. Enforce when committing changes in an event store to handle concurrency.
/// </summary>
public readonly struct StreamExpectation // TODO(fpion): unit tests
{
  /// <summary>
  /// Defines the kinds of stream expectations.
  /// </summary>
  public enum StreamExpectationKind
  {
    /// <summary>
    /// The stream state can be in any state.
    /// </summary>
    None = 0,

    /// <summary>
    /// The stream should already exist before appending the events.
    /// </summary>
    ShouldExist = 1,

    /// <summary>
    /// The stream should not exist and will be created while appending the events.
    /// </summary>
    ShouldNotExist = 2,

    /// <summary>
    /// The stream should be at the specified version after appending the events.
    /// </summary>
    ShouldBeAtVersion = 3
  }

  /// <summary>
  /// Gets the expectation kind.
  /// </summary>
  public StreamExpectationKind Kind { get; }
  /// <summary>
  /// Gets the expected version, when <see cref="Kind"/> is <see cref="StreamExpectationKind.ShouldBeAtVersion"/>.
  /// </summary>
  public long Version { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamExpectation"/> struct. The stream can be in any state.
  /// </summary>
  public StreamExpectation()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamExpectation"/> struct.
  /// </summary>
  /// <param name="kind">The expectation kind.</param>
  /// <exception cref="ArgumentException">The expectation kind was <see cref="StreamExpectationKind.ShouldBeAtVersion"/>.</exception>
  public StreamExpectation(StreamExpectationKind kind)
  {
    if (kind == StreamExpectationKind.ShouldBeAtVersion)
    {
      throw new ArgumentException($"The stream expectation kind cannot be {StreamExpectationKind.ShouldBeAtVersion} when using this constructor.", nameof(kind));
    }

    Kind = kind;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="StreamExpectation"/> struct.
  /// </summary>
  /// <param name="version">The expected stream version after appending the events.</param>
  /// <exception cref="ArgumentOutOfRangeException">The expected version was less than or equal to 0.</exception>
  public StreamExpectation(long version)
  {
    if (version < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(version), "The expected version should be greater than 0.");
    }

    Kind = StreamExpectationKind.ShouldBeAtVersion;
    Version = version;
  }

  /// <summary>
  /// Returns a stream expectation expecting any stream state.
  /// </summary>
  public static StreamExpectation None => new();
  /// <summary>
  /// Returns a stream expectation expecting the stream to exist before appending the events.
  /// </summary>
  public static StreamExpectation ShouldExist => new(StreamExpectationKind.ShouldExist);
  /// <summary>
  /// Returns a stream expectation expecting the stream not to exist before appending the events.
  /// </summary>
  public static StreamExpectation ShouldNotExist => new(StreamExpectationKind.ShouldNotExist);
  /// <summary>
  /// Returns a stream expectation expecting the stream to be at the specified version after appending the events.
  /// </summary>
  /// <param name="version">The expected version.</param>
  /// <returns>The stream expectation.</returns>
  public static StreamExpectation ShouldBeAtVersion(long version) => new(version);

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
