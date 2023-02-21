using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Roles;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Queriers;

/// <summary>
/// Implements methods used to query role read models.
/// </summary>
internal class RoleQuerier : IRoleQuerier
{
  /// <summary>
  /// The mapper instance.
  /// </summary>
  private readonly IMapper _mapper;
  /// <summary>
  /// The data set of roles.
  /// </summary>
  private readonly DbSet<RoleEntity> _roles;

  /// <summary>
  /// Initializes a new instance of the <see cref="RoleQuerier"/> class.
  /// </summary>
  /// <param name="context">The identity context.</param>
  /// <param name="mapper">The mapper instance.</param>
  public RoleQuerier(IdentityContext context, IMapper mapper)
  {
    _mapper = mapper;
    _roles = context.Roles;
  }

  /// <summary>
  /// Retrieves a role by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The role or null if not found.</returns>
  public async Task<Role?> GetAsync(AggregateId id, CancellationToken cancellationToken)
  {
    RoleEntity? role = await _roles.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);

    return _mapper.Map<Role>(role);
  }

  /// <summary>
  /// Retrieves a role by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The role or null if not found.</returns>
  public async Task<Role?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync(new AggregateId(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves a role by its realm and unique name.
  /// </summary>
  /// <param name="realm">The identifier or unique name of the realm in which to search the unique name.</param>
  /// <param name="uniqueName">The unique name.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The role or null if not found.</returns>
  public async Task<Role?> GetAsync(string realm, string uniqueName, CancellationToken cancellationToken)
  {
    string aggregateId = (Guid.TryParse(realm, out Guid realmId)
      ? new AggregateId(realmId)
      : new AggregateId(realm)).Value;

    RoleEntity? role = await _roles.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => (x.Realm!.AggregateId == aggregateId || x.Realm.UniqueNameNormalized == realm.ToUpper())
        && x.UniqueNameNormalized == uniqueName.ToUpper(), cancellationToken);

    return _mapper.Map<Role>(role);
  }

  /// <summary>
  /// Retrieves a list of roles using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="realm">The identifier or unique name of the realm to filter by.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of roles to skip.</param>
  /// <param name="take">The number of roles to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of roles, or empty if none found.</returns>
  public async Task<PagedList<Role>> GetAsync(string? realm, string? search, RoleSort? sort,
    bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    IQueryable<RoleEntity> query = _roles.AsNoTracking()
      .Include(x => x.Realm);

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

          query = query.Where(x => EF.Functions.ILike(x.UniqueName, pattern)
            || (x.DisplayName != null && EF.Functions.ILike(x.DisplayName, pattern)));
        }
      }
    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      switch (sort.Value)
      {
        case RoleSort.DisplayName:
          query = isDescending ? query.OrderByDescending(x => x.DisplayName ?? x.UniqueName) : query.OrderBy(x => x.DisplayName ?? x.UniqueName);
          break;
        case RoleSort.UniqueName:
          query = isDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName);
          break;
        case RoleSort.UpdatedOn:
          query = isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn);
          break;
      }
    }

    query = query.ApplyPaging(skip, take);

    RoleEntity[] roles = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Role>(_mapper.Map<IEnumerable<Role>>(roles), total);
  }
}
