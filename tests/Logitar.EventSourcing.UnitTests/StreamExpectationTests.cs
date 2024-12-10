namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class StreamExpectationTests
{
  [Fact(DisplayName = "ctor: it should construct the correct default stream expectation.")]
  public void Given_DefaultArguments_When_ctor_Then_DefaultExpectation()
  {
    StreamExpectation expectation = new();

    Assert.Equal(StreamExpectation.StreamExpectationKind.None, expectation.Kind);
    Assert.Equal(0, expectation.Version);
  }

  [Theory(DisplayName = "ctor: it should construct the correct stream expectation with kind.")]
  [InlineData(StreamExpectation.StreamExpectationKind.ShouldExist)]
  [InlineData(StreamExpectation.StreamExpectationKind.ShouldNotExist)]
  public void Given_StreamExpectationKind_When_ctor_Then_ExpectationWithKind(StreamExpectation.StreamExpectationKind kind)
  {
    StreamExpectation expectation = new(kind);

    Assert.Equal(kind, expectation.Kind);
    Assert.Equal(0, expectation.Version);
  }

  [Theory(DisplayName = "ctor: it should construct the correct stream expectation with version.")]
  [InlineData(4)]
  public void Given_StreamExpectationKind_When_ctor_Then_ExpectationWithVersion(long version)
  {
    StreamExpectation expectation = new(version);

    Assert.Equal(StreamExpectation.StreamExpectationKind.ShouldBeAtVersion, expectation.Kind);
    Assert.Equal(version, expectation.Version);
  }

  [Fact(DisplayName = "ctor: it should throw ArgumentOutOfRangeException when the kind is ShouldBeAtVersion.")]
  public void Given_ShouldBeAtVersion_When_ctor_Then_ArgumentOutOfRangeException()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new StreamExpectation(StreamExpectation.StreamExpectationKind.ShouldBeAtVersion));
    Assert.StartsWith("When expecting the stream to be at a specific version, use the constructor specifying a version.", exception.Message);
    Assert.Equal("kind", exception.ParamName);
  }

  [Theory(DisplayName = "ctor: it should throw ArgumentOutOfRangeException when the version is less than or equal to 0.")]
  [InlineData(0)]
  [InlineData(-1)]
  public void Given_ZeroOrNegativeVersion_When_ctor_Then_ArgumentOutOfRangeException(long version)
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new StreamExpectation(version));
    Assert.StartsWith("The version should be greater than 0.", exception.Message);
    Assert.Equal("version", exception.ParamName);
  }

  [Fact(DisplayName = "EqualOperator: it should return false when the expectations are different.")]
  public void Given_DifferentExpectations_When_EqualOperator_Then_False()
  {
    StreamExpectation expectation1 = new(version: 10);
    StreamExpectation expectation2 = StreamExpectation.ShouldNotExist;
    Assert.False(expectation1 == expectation2);
  }

  [Fact(DisplayName = "EqualOperator: it should return true when the expectations are equal.")]
  public void Given_SameExpectations_When_EqualOperator_Then_True()
  {
    StreamExpectation expectation1 = new(version: 10);
    StreamExpectation expectation2 = StreamExpectation.ShouldBeAtVersion(expectation1.Version);
    Assert.True(expectation1 == expectation2);
  }

  [Fact(DisplayName = "Equals: it should return false when comparing different expectations.")]
  public void Given_DifferentExpectations_When_Equals_Then_False()
  {
    StreamExpectation expectation1 = new(version: 10);
    StreamExpectation expectation2 = StreamExpectation.ShouldNotExist;
    Assert.False(expectation1.Equals(expectation2));
  }

  [Fact(DisplayName = "Equals: it should return false when comparing different objects.")]
  public void Given_DifferentTypes_When_Equals_Then_False()
  {
    StreamExpectation expectation = new(version: 10);
    User user = new();
    Assert.False(expectation.Equals(user));
  }

  [Fact(DisplayName = "Equals: it should return true when comparing the same expectation.")]
  public void Given_SameExpectation_When_Equals_Then_True()
  {
    StreamExpectation expectation1 = new(version: 10);
    StreamExpectation expectation2 = new(expectation1.Version);
    Assert.True(expectation1.Equals(expectation1));
    Assert.True(expectation1.Equals(expectation2));
  }

  [Fact(DisplayName = "Equals: it should return false when comparing with null.")]
  public void Given_Null_When_Equals_Then_False()
  {
    StreamExpectation expectation = new(version: 10);
    Assert.False(expectation.Equals(null));
  }

  [Fact(DisplayName = "None: it should return the correct stream expectation.")]
  public void Given_StreamExpectation_When_None_Then_CorrectExpectation()
  {
    StreamExpectation expectation = StreamExpectation.None;

    Assert.Equal(StreamExpectation.StreamExpectationKind.None, expectation.Kind);
    Assert.Equal(0, expectation.Version);
  }

  [Fact(DisplayName = "NotEqualOperator: it should return false when the expectations are equal.")]
  public void Given_SameExpectations_When_NotEqualOperator_Then_False()
  {
    StreamExpectation expectation1 = new(version: 10);
    StreamExpectation expectation2 = StreamExpectation.ShouldBeAtVersion(expectation1.Version);
    Assert.False(expectation1 != expectation2);
  }

  [Fact(DisplayName = "NotEqualOperator: it should return true when the expectations are different.")]
  public void Given_DifferentExpectations_When_NotEqualOperator_Then_True()
  {
    StreamExpectation expectation1 = new(version: 10);
    StreamExpectation expectation2 = StreamExpectation.ShouldNotExist;
    Assert.True(expectation1 != expectation2);
  }

  [Theory(DisplayName = "ShouldBeAtVersion: it should return the correct stream expectation.")]
  [InlineData(7)]
  public void Given_StreamExpectation_When_ShouldBeAtVersion_Then_CorrectExpectation(long version)
  {
    StreamExpectation expectation = StreamExpectation.ShouldBeAtVersion(version);

    Assert.Equal(StreamExpectation.StreamExpectationKind.ShouldBeAtVersion, expectation.Kind);
    Assert.Equal(version, expectation.Version);
  }

  [Fact(DisplayName = "ShouldExist: it should return the correct stream expectation.")]
  public void Given_StreamExpectation_When_ShouldExist_Then_CorrectExpectation()
  {
    StreamExpectation expectation = StreamExpectation.ShouldExist;

    Assert.Equal(StreamExpectation.StreamExpectationKind.ShouldExist, expectation.Kind);
    Assert.Equal(0, expectation.Version);
  }

  [Fact(DisplayName = "ShouldNotExist: it should return the correct stream expectation.")]
  public void Given_StreamExpectation_When_ShouldNotExist_Then_CorrectExpectation()
  {
    StreamExpectation expectation = StreamExpectation.ShouldNotExist;

    Assert.Equal(StreamExpectation.StreamExpectationKind.ShouldNotExist, expectation.Kind);
    Assert.Equal(0, expectation.Version);
  }

  [Theory(DisplayName = "GetHashCode: it should return the correct hash code.")]
  [InlineData(null)]
  [InlineData(2L)]
  public void Given_Expectation_When_GetHashCode_Then_CorrectHashCode(long? version)
  {
    StreamExpectation expectation = version.HasValue ? StreamExpectation.ShouldBeAtVersion(version.Value) : StreamExpectation.None;
    Assert.Equal(HashCode.Combine(expectation.Kind, expectation.Version), expectation.GetHashCode());
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation.")]
  [InlineData(null)]
  [InlineData(11L)]
  public void Given_Expectation_When_ToString_Then_CorrectStringRepresentation(long? version)
  {
    if (version.HasValue)
    {
      StreamExpectation expectation = StreamExpectation.ShouldBeAtVersion(version.Value);
      Assert.Equal(string.Format("ShouldBeAtVersion: {0}", version), expectation.ToString());
    }
    else
    {
      StreamExpectation expectation = StreamExpectation.None;
      Assert.Equal("None", expectation.ToString());
    }
  }
}
