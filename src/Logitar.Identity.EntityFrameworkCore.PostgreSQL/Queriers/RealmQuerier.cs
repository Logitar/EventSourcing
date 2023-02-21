using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Realms;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Queriers;

/// <summary>
/// Implements methods used to query realm read models.
/// </summary>
internal class RealmQuerier : IRealmQuerier
{
  /// <summary>
  /// The mapper instance.
  /// </summary>
  private readonly IMapper _mapper;
  /// <summary>
  /// The data set of realms.
  /// </summary>
  private readonly DbSet<RealmEntity> _realms;

  /// <summary>
  /// Initializes a new instance of the <see cref="RealmQuerier"/> class.
  /// </summary>
  /// <param name="context">The identity context.</param>
  /// <param name="mapper">The mapper instance.</param>
  public RealmQuerier(IdentityContext context, IMapper mapper)
  {
    _mapper = mapper;
    _realms = context.Realms;
  }

  /// <summary>
  /// Retrieves a realm by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm or null if not found.</returns>
  public async Task<Realm?> GetAsync(AggregateId id, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);

    return _mapper.Map<Realm>(realm);
  }

  /// <summary>
  /// Retrieves a realm by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm or null if not found.</returns>
  public async Task<Realm?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync(new AggregateId(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves a realm by its unique name.
  /// </summary>
  /// <param name="uniqueName">The unique name.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm or null if not found.</returns>
  public async Task<Realm?> GetAsync(string uniqueName, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueName.ToUpper(), cancellationToken);

    return _mapper.Map<Realm>(realm);
  }

  /// <summary>
  /// Retrieves a list of realms using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="search">The text to search.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of realms to skip.</param>
  /// <param name="take">The number of realms to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of realms, or empty if none found.</returns>
  public async Task<PagedList<Realm>> GetAsync(string? search, RealmSort? sort, bool isDescending,
    int? skip, int? take, CancellationToken cancellationToken)
  {
    IQueryable<RealmEntity> query = _realms.AsNoTracking();

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
        case RealmSort.DisplayName:
          query = isDescending ? query.OrderByDescending(x => x.DisplayName ?? x.UniqueName) : query.OrderBy(x => x.DisplayName ?? x.UniqueName);
          break;
        case RealmSort.UniqueName:
          query = isDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName);
          break;
        case RealmSort.UpdatedOn:
          query = isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn);
          break;
      }
    }

    query = query.ApplyPaging(skip, take);

    RealmEntity[] realms = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Realm>(_mapper.Map<IEnumerable<Realm>>(realms), total);
  }
}
