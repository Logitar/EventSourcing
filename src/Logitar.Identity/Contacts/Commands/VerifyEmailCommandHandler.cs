using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The handler for <see cref="VerifyEmailCommand"/> commands.
/// </summary>
internal class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Email>
{
  /// <summary>
  /// The actor context.
  /// </summary>
  private readonly IActorContext _actorContext;
  /// <summary>
  /// The email address querier.
  /// </summary>
  private readonly IEmailQuerier _emailQuerier;
  /// <summary>
  /// The event store.
  /// </summary>
  private readonly IEventStore _eventStore;

  /// <summary>
  /// Initializes a new instance of the <see cref="VerifyEmailCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// /// <param name="actorContext">The actor context.</param>
  /// <param name="emailQuerier">The email address querier.</param>
  /// <param name="eventStore">The event store.</param>
  public VerifyEmailCommandHandler(IActorContext actorContext,
    IEmailQuerier emailQuerier,
    IEventStore eventStore)
  {
    _actorContext = actorContext;
    _emailQuerier = emailQuerier;
    _eventStore = eventStore;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The verified email address.</returns>
  /// <exception cref="AggregateNotFoundException{EmailAggregate}">The specified email address could not be found.</exception>
  /// <exception cref="InvalidOperationException">The email address output could not be found.</exception>
  public async Task<Email> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    EmailAggregate email = await _eventStore.LoadAsync<EmailAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<EmailAggregate>(id);

    email.Verify(_actorContext.ActorId);

    await _eventStore.SaveAsync(email, cancellationToken);

    return await _emailQuerier.GetAsync(email.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The email address output (Id={email.Id}) could not be found.");
  }
}
