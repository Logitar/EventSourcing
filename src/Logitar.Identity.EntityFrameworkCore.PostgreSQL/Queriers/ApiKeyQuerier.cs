using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.ApiKeys;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Queriers;

/// <summary>
/// Implements methods used to query API key read models.
/// </summary>
internal class ApiKeyQuerier : IApiKeyQuerier
{
  /// <summary>
  /// The data set of API keys.
  /// </summary>
  private readonly DbSet<ApiKeyEntity> _apiKeys;
  /// <summary>
  /// The mapper instance.
  /// </summary>
  private readonly IMapper _mapper;

  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyQuerier"/> class.
  /// </summary>
  /// <param name="context">The identity context.</param>
  /// <param name="mapper">The mapper instance.</param>
  public ApiKeyQuerier(IdentityContext context, IMapper mapper)
  {
    _apiKeys = context.ApiKeys;
    _mapper = mapper;
  }

  /// <summary>
  /// Retrieves a API key by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The API key or null if not found.</returns>
  public async Task<ApiKey?> GetAsync(AggregateId id, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await _apiKeys.AsNoTracking()
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);

    return _mapper.Map<ApiKey>(apiKey);
  }

  /// <summary>
  /// Retrieves a API key by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The API key or null if not found.</returns>
  public async Task<ApiKey?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync(new AggregateId(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves a list of API keys using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="realm">The identifier or unique name of the realm to filter by.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of API keys to skip.</param>
  /// <param name="take">The number of API keys to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of API keys, or empty if none found.</returns>
  public async Task<PagedList<ApiKey>> GetAsync(string? realm, string? search, ApiKeySort? sort, bool isDescending,
    int? skip, int? take, CancellationToken cancellationToken)
  {
    IQueryable<ApiKeyEntity> query = _apiKeys.AsNoTracking()
      .Include(x => x.Roles);

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

          query = query.Where(x => EF.Functions.ILike(x.Title, pattern));
        }
      }
    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      switch (sort.Value)
      {
        case ApiKeySort.ExpiresOn:
          query = isDescending ? query.OrderByDescending(x => x.ExpiresOn) : query.OrderBy(x => x.ExpiresOn);
          break;
        case ApiKeySort.Title:
          query = isDescending ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title);
          break;
        case ApiKeySort.UpdatedOn:
          query = isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn);
          break;
      }
    }

    query = query.ApplyPaging(skip, take);

    ApiKeyEntity[] apiKeys = await query.ToArrayAsync(cancellationToken);

    return new PagedList<ApiKey>(_mapper.Map<IEnumerable<ApiKey>>(apiKeys), total);
  }
}
