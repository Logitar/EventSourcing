﻿namespace Logitar.Identity.Domain.Users;

public class UserIsNotConfirmedException : Exception
{
  public UserIsNotConfirmedException(UserAggregate user) : base($"The user 'Id={user.Id}' is not confirmed.")
  {
    User = user.ToString();
  }

  public string User
  {
    get => (string)Data[nameof(User)]!;
    private set => Data[nameof(User)] = value;
  }
}