using Logitar.EventSourcing;
using Logitar.Identity.Users;
using MediatR;

namespace Logitar.Identity.Contacts.Commands;

/// <summary>
/// The handler for <see cref="CreateEmailCommand"/> commands.
/// </summary>
internal class CreateEmailCommandHandler : IRequestHandler<CreateEmailCommand, Email>
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
  /// Initializes a new instance of the <see cref="CreateEmailCommandHandler"/> class using the specified arguments.
  /// </summary>
  /// <param name="actorContext">The actor context.</param>
  /// <param name="emailQuerier">The email address querier.</param>
  /// <param name="emailRepository">The email address repository.</param>
  /// <param name="eventStore">The event store.</param>
  public CreateEmailCommandHandler(IActorContext actorContext,
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
  /// <returns>The created email address.</returns>
  /// <exception cref="AggregateNotFoundException">The specified user could not be found.</exception>
  /// <exception cref="InvalidOperationException">The email address output could not be found.</exception>
  public async Task<Email> Handle(CreateEmailCommand command, CancellationToken cancellationToken)
  {
    CreateEmailInput input = command.Input;

    AggregateId userId = new(input.UserId);
    UserAggregate user = await _eventStore.LoadAsync<UserAggregate>(userId, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(userId, nameof(input.UserId));

    List<EmailAggregate> changedEmails = new(capacity: 2);
    bool isDefault = input.IsDefault;
    if (isDefault)
    {
      EmailAggregate? @default = await _emailRepository.LoadDefaultAsync(user.Id, cancellationToken);
      if (@default != null)
      {
        @default.SetDefault(_actorContext.ActorId, isDefault: false);
        changedEmails.Add(@default);
      }
    }
    else
    {
      isDefault = !(await _emailRepository.LoadByUserAsync(user.Id, cancellationToken)).Any();
    }

    Dictionary<string, string>? customAttributes = input.CustomAttributes?.ToDictionary();

    EmailAggregate email = new(_actorContext.ActorId, user, input.Address, isLogin: false, // TODO(fpion): implement
      input.IsArchived, isDefault, input.IsVerified, input.Label, customAttributes);
    changedEmails.Add(email);

    await _eventStore.SaveAsync(changedEmails, cancellationToken);

    return await _emailQuerier.GetAsync(email.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The email address output (Id={email.Id}) could not be found.");
  }
}
