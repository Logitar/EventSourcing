namespace Logitar.EventSourcing;

internal class User;

internal record UserCreated(string UniqueName) : DomainEvent;

internal record UserPasswordCreated(string PasswordHash) : DomainEvent;

internal record UserSignedIn : DomainEvent;
