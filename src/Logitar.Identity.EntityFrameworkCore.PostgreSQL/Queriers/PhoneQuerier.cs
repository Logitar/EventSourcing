using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.Contacts;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Queriers;

/// <summary>
/// Implements methods used to query phone number read models.
/// </summary>
internal class PhoneQuerier : IPhoneQuerier
{
  /// <summary>
  /// The mapper instance.
  /// </summary>
  private readonly IMapper _mapper;
  /// <summary>
  /// The data set of phone numbers.
  /// </summary>
  private readonly DbSet<PhoneEntity> _phones;

  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneQuerier"/> class.
  /// </summary>
  /// <param name="context">The identity context.</param>
  /// <param name="mapper">The mapper instance.</param>
  public PhoneQuerier(IdentityContext context, IMapper mapper)
  {
    _mapper = mapper;
    _phones = context.Phones;
  }

  /// <summary>
  /// Retrieves a phone number by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The phone number or null if not found.</returns>
  public async Task<Phone?> GetAsync(AggregateId id, CancellationToken cancellationToken)
  {
    PhoneEntity? phone = await _phones.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);

    return _mapper.Map<Phone>(phone);
  }

  /// <summary>
  /// Retrieves a phone number by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The phone number or null if not found.</returns>
  public async Task<Phone?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync(new AggregateId(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves the default phone number of the specified user.
  /// </summary>
  /// <param name="id">The identifier of the user to retrieve the default phone number.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The phone number or null if not found.</returns>
  public async Task<Phone?> GetDefaultAsync(Guid userId, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(userId).Value;

    PhoneEntity? phone = await _phones.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Realm)
      .SingleOrDefaultAsync(x => x.IsDefault && x.User!.AggregateId == aggregateId, cancellationToken);

    return _mapper.Map<Phone>(phone);
  }

  /// <summary>
  /// Retrieves a list of phone numbers using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="isArchived">The value filtering phone numbers on their archivation status.</param>
  /// <param name="isVerified">The value filtering phone numbers on their verification status.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="userId">The identifier of the user to filter by.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of phone numbers to skip.</param>
  /// <param name="take">The number of phone numbers to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of phone numbers, or empty if none found.</returns>
  public async Task<PagedList<Phone>> GetAsync(bool? isArchived, bool? isVerified, string? search, Guid? userId,
    PhoneSort? sort, bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    IQueryable<PhoneEntity> query = _phones.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Realm);

    if (isArchived.HasValue)
    {
      query = query.Where(x => x.IsArchived == isArchived.Value);
    }
    if (isVerified.HasValue)
    {
      query = query.Where(x => x.IsVerified == isVerified.Value);
    }
    if (search != null)
    {
      foreach (string term in search.Split())
      {
        if (!string.IsNullOrEmpty(term))
        {
          string pattern = $"%{term}%";

          query = query.Where(x => EF.Functions.ILike(x.Number, pattern)
            || (x.CountryCode != null && EF.Functions.ILike(x.CountryCode, pattern))
            || (x.Extension != null && EF.Functions.ILike(x.Extension, pattern))
            || (x.Label != null && EF.Functions.ILike(x.Label, pattern)));
        }
      }
    }
    if (userId.HasValue)
    {
      string aggregateId = new AggregateId(userId.Value).Value;
      query = query.Where(x => x.User!.AggregateId == aggregateId);
    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      switch (sort.Value)
      {
        case PhoneSort.Label:
          query = isDescending ? query.OrderByDescending(x => x.Label) : query.OrderBy(x => x.Label);
          break;
        case PhoneSort.Number:
          query = isDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number);
          break;
        case PhoneSort.UpdatedOn:
          query = isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn);
          break;
        case PhoneSort.VerifiedOn:
          query = isDescending ? query.OrderByDescending(x => x.VerifiedOn) : query.OrderBy(x => x.VerifiedOn);
          break;
      }
    }

    query = query.ApplyPaging(skip, take);

    PhoneEntity[] phones = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Phone>(_mapper.Map<IEnumerable<Phone>>(phones), total);
  }
}
