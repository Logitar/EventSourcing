using System.Security.Cryptography;

namespace Logitar.EventSourcing;

internal sealed class Token : IAggregate
{
  public StreamId Id { get; private set; }

  private readonly List<IEvent> _changes = [];
  public bool HasChanges => _changes.Count > 0;
  public IReadOnlyCollection<IEvent> Changes => _changes.AsReadOnly();
  public void ClearChanges()
  {
    _changes.Clear();
  }

  public string Hash { get; private set; } = string.Empty;

  public Token()
  {
    Id = StreamId.NewId();
  }

  public Token(StreamId id)
  {
    Id = id;
  }

  public static Token Generate()
  {
    byte[] bytes = RandomNumberGenerator.GetBytes(256 / 8);
    string hash = Convert.ToBase64String(bytes);

    Token token = new();
    token.Raise(new TokenGenerated(hash));
    return token;
  }

  public void LoadFromChanges(StreamId id, IEnumerable<IEvent> changes)
  {
    Id = id;

    foreach (IEvent change in changes)
    {
      Apply(change);
    }
  }

  private void Raise(IEvent change)
  {
    Apply(change);

    _changes.Add(change);
  }
  private void Apply(IEvent change)
  {
    if (change is TokenGenerated generated)
    {
      Hash = generated.Hash;
    }
  }
}

internal record TokenGenerated(string Hash) : IEvent;
