using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.Contacts;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Queriers;

/// <summary>
/// Implements methods used to query postal address read models.
/// </summary>
internal class AddressQuerier : IAddressQuerier
{
  /// <summary>
  /// The data set of postal addresses.
  /// </summary>
  private readonly DbSet<AddressEntity> _addresses;
  /// <summary>
  /// The mapper instance.
  /// </summary>
  private readonly IMapper _mapper;

  /// <summary>
  /// Initializes a new instance of the <see cref="AddressQuerier"/> class.
  /// </summary>
  /// <param name="context">The identity context.</param>
  /// <param name="mapper">The mapper instance.</param>
  public AddressQuerier(IdentityContext context, IMapper mapper)
  {
    _addresses = context.Addresses;
    _mapper = mapper;
  }

  /// <summary>
  /// Retrieves a postal address by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The postal address or null if not found.</returns>
  public async Task<Address?> GetAsync(AggregateId id, CancellationToken cancellationToken)
  {
    AddressEntity? address = await _addresses.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);

    return _mapper.Map<Address>(address);
  }

  /// <summary>
  /// Retrieves a postal address by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The postal address or null if not found.</returns>
  public async Task<Address?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync(new AggregateId(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves the default postal address of the specified user.
  /// </summary>
  /// <param name="id">The identifier of the user to retrieve the default postal address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The postal address or null if not found.</returns>
  public async Task<Address?> GetDefaultAsync(Guid userId, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(userId).Value;

    AddressEntity? address = await _addresses.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Realm)
      .SingleOrDefaultAsync(x => x.IsDefault && x.User!.AggregateId == aggregateId, cancellationToken);

    return _mapper.Map<Address>(address);
  }

  /// <summary>
  /// Retrieves a list of postal addresses using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="isArchived">The value filtering postal addresses on their archivation status.</param>
  /// <param name="isVerified">The value filtering postal addresses on their verification status.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="userId">The identifier of the user to filter by.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of postal addresses to skip.</param>
  /// <param name="take">The number of postal addresses to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of postal addresses, or empty if none found.</returns>
  public async Task<PagedList<Address>> GetAsync(bool? isArchived, bool? isVerified, string? search, Guid? userId,
    AddressSort? sort, bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    IQueryable<AddressEntity> query = _addresses.AsNoTracking()
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

          query = query.Where(x => EF.Functions.ILike(x.Line1, pattern)
            || EF.Functions.ILike(x.Locality, pattern)
            || EF.Functions.ILike(x.Country, pattern)
            || (x.Line2 != null && EF.Functions.ILike(x.Line2, pattern))
            || (x.PostalCode != null && EF.Functions.ILike(x.PostalCode, pattern))
            || (x.Region != null && EF.Functions.ILike(x.Region, pattern))
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
        case AddressSort.Country:
          query = isDescending ? query.OrderByDescending(x => x.Country) : query.OrderBy(x => x.Country);
          break;
        case AddressSort.Label:
          query = isDescending ? query.OrderByDescending(x => x.Label) : query.OrderBy(x => x.Label);
          break;
        case AddressSort.Line1:
          query = isDescending ? query.OrderByDescending(x => x.Line1) : query.OrderBy(x => x.Line1);
          break;
        case AddressSort.Locality:
          query = isDescending ? query.OrderByDescending(x => x.Locality) : query.OrderBy(x => x.Locality);
          break;
        case AddressSort.UpdatedOn:
          query = isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn);
          break;
        case AddressSort.VerifiedOn:
          query = isDescending ? query.OrderByDescending(x => x.VerifiedOn) : query.OrderBy(x => x.VerifiedOn);
          break;
      }
    }

    query = query.ApplyPaging(skip, take);

    AddressEntity[] addresses = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Address>(_mapper.Map<IEnumerable<Address>>(addresses), total);
  }
}
