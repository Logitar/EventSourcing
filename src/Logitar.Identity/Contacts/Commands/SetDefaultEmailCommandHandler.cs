using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The handler for <see cref="SetDefaultEmailCommand"/> commands.
/// </summary>
internal class SetDefaultEmailCommandHandler : IRequestHandler<SetDefaultEmailCommand, Email>
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
  /// The email address repository.
  /// </summary>
  private readonly IEmailRepository _emailRepository;
  /// <summary>
  /// The event store.
  /// </summary>
  private readonly IEventStore _eventStore;

  /// <summary>
  /// Initializes a new instance of the <see cref="SetDefaultEmailCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// /// <param name="actorContext">The actor context.</param>
  /// <param name="emailQuerier">The email address querier.</param>
  /// <param name="emailRepository">The email address repository.</param>
  /// <param name="eventStore">The event store.</param>
  public SetDefaultEmailCommandHandler(IActorContext actorContext,
    IEmailQuerier emailQuerier,
    IEmailRepository emailRepository,
    IEventStore eventStore)
  {
    _actorContext = actorContext;
    _emailQuerier = emailQuerier;
    _emailRepository = emailRepository;
    _eventStore = eventStore;
  }

  /// <summary>
  /// Handles the specified command instance.
  /// </summary>
  /// <param name="command">The command to handle.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The default email address.</returns>
  /// <exception cref="AggregateNotFoundException{EmailAggregate}">The specified email address could not be found.</exception>
  /// <exception cref="InvalidOperationException">The email address output could not be found.</exception>
  public async Task<Email> Handle(SetDefaultEmailCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = new(command.Id);
    EmailAggregate email = await _eventStore.LoadAsync<EmailAggregate>(id, cancellationToken)
      ?? throw new AggregateNotFoundException<EmailAggregate>(id);

    List<EmailAggregate> changedEmails = new(capacity: 2);

    EmailAggregate? @default = await _emailRepository.LoadDefaultAsync(email.UserId, cancellationToken);
    if (@default != null)
    {
      @default.SetDefault(_actorContext.ActorId, isDefault: false);
      changedEmails.Add(@default);
    }

    email.SetDefault(_actorContext.ActorId);
    changedEmails.Add(email);

    await _eventStore.SaveAsync(changedEmails, cancellationToken);

    return await _emailQuerier.GetAsync(email.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The email address output (Id={email.Id}) could not be found.");
  }
}
