﻿using Bogus;

namespace Logitar.EventSourcing;

[Trait(Traits.Category, Categories.Unit)]
public class StreamIdTests
{
  private readonly Faker _faker = new();

  private readonly StreamId _id = StreamId.NewId();

  [Theory(DisplayName = "Ctor: it constructs the correct Guid identifier.")]
  [InlineData("00000000-0000-0000-0000-000000000000")]
  [InlineData("47a6c4e5-08d9-4eb6-8d31-f82b16ba0562")]
  public void Ctor_it_constructs_the_correct_Guid_identifier(string value)
  {
    Guid guid = Guid.Parse(value);
    StreamId id = new(guid);
    string idValue = Convert.ToBase64String(guid.ToByteArray()).ToUriSafeBase64();
    Assert.Equal(idValue, id.Value);
  }

  [Theory(DisplayName = "Ctor: it constructs the correct string identifier.")]
  [InlineData("123456")]
  [InlineData("  123456  ")]
  public void Ctor_it_constructs_the_correct_string_identifier(string value)
  {
    StreamId id = new(value);
    Assert.Equal(value.Trim(), id.Value);
  }

  [Theory(DisplayName = "Ctor: it throws ArgumentException when value is null or white space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void Ctor_it_throws_ArgumentException_when_value_is_null_or_white_space(string? value)
  {
    var exception = Assert.Throws<ArgumentException>(() => new StreamId(value!));
    Assert.Equal("value", exception.ParamName);
  }

  [Theory(DisplayName = "Ctor: it throws ArgumentOutOfRangeException when value is too long.")]
  [InlineData(1000)]
  public void Ctor_it_throws_ArgumentOutOfRangeException_when_value_is_too_long(int length)
  {
    string value = _faker.Random.String(length, minChar: 'A', maxChar: 'Z');
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new StreamId(value));
    Assert.Equal("value", exception.ParamName);
  }

  [Fact(DisplayName = "EqualOperator: it returns false when they are different.")]
  public void EqualOperator_it_returns_false_when_they_are_different()
  {
    StreamId id = StreamId.NewId();
    StreamId other = new(id.Value[1..]);
    Assert.False(id == other);
  }

  [Fact(DisplayName = "EqualOperator: it returns true when they are equal.")]
  public void EqualOperator_it_returns_true_when_they_are_equal()
  {
    StreamId id = StreamId.NewId();
    StreamId other = new(id.ToGuid());
    Assert.True(id == other);
  }

  [Fact(DisplayName = "Equals: it returns false when other is a different StreamId.")]
  public void Equals_it_returns_false_when_other_is_a_different_StreamId()
  {
    StreamId other = new(_id.Value[1..]);
    Assert.False(_id.Equals(other));
  }

  [Fact(DisplayName = "Equals: it returns false when other is not an StreamId.")]
  public void Equals_it_returns_false_when_other_is_not_an_StreamId()
  {
    Assert.False(_id.Equals(_id.Value));
  }

  [Fact(DisplayName = "Equals: it returns false when other is null.")]
  public void Equals_it_returns_false_when_other_is_null()
  {
    Assert.False(_id.Equals(null));
  }

  [Fact(DisplayName = "Equals: it returns true when other is equal.")]
  public void Equals_it_returns_false_when_other_is_equal()
  {
    StreamId other = new(_id.Value);
    Assert.True(_id.Equals(other));
  }

  [Fact(DisplayName = "GetHashCode: it returns the correct hash code.")]
  public void GetHashCode_it_returns_the_correct_hash_code()
  {
    Assert.Equal(_id.Value.GetHashCode(), _id.GetHashCode());
  }

  [Fact(DisplayName = "NewId: it is constructed using a Guid.")]
  public void NewId_it_is_constructed_using_a_Guid()
  {
    _ = StreamId.NewId().ToGuid();
  }

  [Fact(DisplayName = "NotEqualOperator: it returns false when they are equal.")]
  public void NotEqualOperator_it_returns_false_when_they_are_equal()
  {
    StreamId id = StreamId.NewId();
    StreamId other = new(id.ToGuid());
    Assert.False(id != other);
  }

  [Fact(DisplayName = "NotEqualOperator: it returns true when they are different.")]
  public void NotEqualOperator_it_returns_true_when_they_are_different()
  {
    StreamId id = StreamId.NewId();
    StreamId other = new(id.Value[1..]);
    Assert.True(id != other);
  }

  [Fact(DisplayName = "ToGuid: it returns the correct Guid.")]
  public void ToGuid_it_returns_the_correct_Guid()
  {
    Guid guid = new(Convert.FromBase64String(_id.Value.FromUriSafeBase64()));
    Assert.Equal(guid, _id.ToGuid());
  }

  [Fact(DisplayName = "ToString: it returns the correct string.")]
  public void ToString_it_returns_the_correct_string()
  {
    Assert.Equal(_id.Value, _id.ToString());
  }

  [Fact(DisplayName = "Value: it should never be null.")]
  public void Value_it_should_never_be_null()
  {
    Assert.NotNull(new StreamId().Value);
  }
}
