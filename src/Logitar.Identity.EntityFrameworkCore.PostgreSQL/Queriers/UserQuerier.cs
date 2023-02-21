using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Users;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Queriers;

/// <summary>
/// Implement methods used to query user read models.
/// </summary>
internal class UserQuerier : IUserQuerier
{
  /// <summary>
  /// The mapper instance.
  /// </summary>
  private readonly IMapper _mapper;
  /// <summary>
  /// The data set of users.
  /// </summary>
  private readonly DbSet<UserEntity> _users;

  /// <summary>
  /// Initializes a new instance of the <see cref="UserQuerier"/> class.
  /// </summary>
  /// <param name="context">The identity context.</param>
  /// <param name="mapper">The mapper instance.</param>
  public UserQuerier(IdentityContext context, IMapper mapper)
  {
    _mapper = mapper;
    _users = context.Users;
  }

  /// <summary>
  /// Retrieves an user by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  public async Task<User?> GetAsync(AggregateId id, CancellationToken cancellationToken)
  {
    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.ExternalIdentifiers)
      .Include(x => x.Realm)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);

    return _mapper.Map<User>(user);
  }

  /// <summary>
  /// Retrieves an user by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync(new AggregateId(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves an user by its realm and unique name.
  /// </summary>
  /// <param name="realm">The identifier or unique name of the realm in which to search the unique name.</param>
  /// <param name="username">The unique name.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  public async Task<User?> GetAsync(string realm, string username, CancellationToken cancellationToken)
  {
    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.ExternalIdentifiers)
      .Include(x => x.Realm)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => (x.Realm!.AggregateId == realm || x.Realm.UniqueNameNormalized == realm.ToUpper())
        && x.UsernameNormalized == username.ToUpper(), cancellationToken);

    return _mapper.Map<User>(user);
  }

  /// <summary>
  /// Retrieves an user by its realm and external identifier.
  /// </summary>
  /// <param name="realm">The identifier or unique name of the realm in which to search the external identifier.</param>
  /// <param name="externalKey">The key of the external identifier.</param>
  /// <param name="externalValue">The value of the external identifieré</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The user or null if not found.</returns>
  public async Task<User?> GetAsync(string realm, string externalKey, string externalValue, CancellationToken cancellationToken)
  {
    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.ExternalIdentifiers)
      .Include(x => x.Realm)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => (x.Realm!.AggregateId == realm || x.Realm.UniqueNameNormalized == realm.ToUpper())
        && x.ExternalIdentifiers.Any(y => y.Key == externalKey && y.Value == externalValue), cancellationToken);

    return _mapper.Map<User>(user);
  }

  /// <summary>
  /// Retrieves a list of users using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="isDisabled">The value filtering users on their disabled status.</param>
  /// <param name="realm">The identifier or unique name of the realm to filter by.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of users to skip.</param>
  /// <param name="take">The number of users to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of users, or empty if none found.</returns>
  public async Task<PagedList<User>> GetAsync(bool? isDisabled, string? realm, string? search,
    UserSort? sort, bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    IQueryable<UserEntity> query = _users.AsNoTracking()
      .Include(x => x.ExternalIdentifiers)
      .Include(x => x.Realm)
      .Include(x => x.Roles);

    if (isDisabled.HasValue)
    {
      query = query.Where(x => x.IsDisabled == isDisabled.Value);
    }
    if (realm != null)
    {
      string aggregateId = (Guid.TryParse(realm, out Guid realmId)
        ? new AggregateId(realmId)
        : new AggregateId(realm)).Value;

      query = query.Where(x => x.Realm!.AggregateId == aggregateId || x.Realm.UniqueNameNormalized == realm.ToUpper());
    }
    if (search != null)
    {
      foreach (string term in search.Split())
      {
        if (!string.IsNullOrEmpty(term))
        {
          string pattern = $"%{term}%";

          query = query.Where(x => EF.Functions.ILike(x.Username, pattern)
            || (x.FullName != null && EF.Functions.ILike(x.FullName, pattern))
            || (x.Nickname != null && EF.Functions.ILike(x.Nickname, pattern)));
        }
      }
    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      switch (sort.Value)
      {
        case UserSort.DisabledOn:
          query = isDescending ? query.OrderByDescending(x => x.DisabledOn) : query.OrderBy(x => x.DisabledOn);
          break;
        case UserSort.FullName:
          query = isDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName);
          break;
        case UserSort.LastFirstMiddleName:
          query = isDescending
            ? query.OrderByDescending(x => x.LastName).ThenByDescending(x => x.FirstName).ThenByDescending(x => x.MiddleName)
            : query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.MiddleName);
          break;
        case UserSort.PasswordChangedOn:
          query = isDescending ? query.OrderByDescending(x => x.PasswordChangedOn) : query.OrderBy(x => x.PasswordChangedOn);
          break;
        case UserSort.SignedInOn:
          query = isDescending ? query.OrderByDescending(x => x.SignedInOn) : query.OrderBy(x => x.SignedInOn);
          break;
        case UserSort.UpdatedOn:
          query = isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn);
          break;
        case UserSort.Username:
          query = isDescending ? query.OrderByDescending(x => x.Username) : query.OrderBy(x => x.Username);
          break;
      }
    }

    query = query.ApplyPaging(skip, take);

    UserEntity[] users = await query.ToArrayAsync(cancellationToken);

    return new PagedList<User>(_mapper.Map<IEnumerable<User>>(users), total);
  }
}
