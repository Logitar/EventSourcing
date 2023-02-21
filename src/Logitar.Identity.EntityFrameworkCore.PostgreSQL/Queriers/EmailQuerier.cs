using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Identity.Contacts;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Queriers;

/// <summary>
/// Implements methods used to query email address read models.
/// </summary>
internal class EmailQuerier : IEmailQuerier
{
  /// <summary>
  /// The data set of email addresses.
  /// </summary>
  private readonly DbSet<EmailEntity> _emails;
  /// <summary>
  /// The mapper instance.
  /// </summary>
  private readonly IMapper _mapper;

  /// <summary>
  /// Initializes a new instance of the <see cref="EmailQuerier"/> class.
  /// </summary>
  /// <param name="context">The identity context.</param>
  /// <param name="mapper">The mapper instance.</param>
  public EmailQuerier(IdentityContext context, IMapper mapper)
  {
    _emails = context.Emails;
    _mapper = mapper;
  }

  /// <summary>
  /// Retrieves an email address by its aggregate identifier.
  /// </summary>
  /// <param name="id">The aggregate identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The email address or null if not found.</returns>
  public async Task<Email?> GetAsync(AggregateId id, CancellationToken cancellationToken)
  {
    EmailEntity? email = await _emails.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);

    return _mapper.Map<Email>(email);
  }

  /// <summary>
  /// Retrieves an email address by its <see cref="Guid"/>.
  /// </summary>
  /// <param name="id">The Guid.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The email address or null if not found.</returns>
  public async Task<Email?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync(new AggregateId(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves the default email address of the specified user.
  /// </summary>
  /// <param name="id">The identifier of the user to retrieve the default email address.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The email address or null if not found.</returns>
  public async Task<Email?> GetDefaultAsync(Guid userId, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(userId).Value;

    EmailEntity? email = await _emails.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Realm)
      .SingleOrDefaultAsync(x => x.IsDefault && x.User!.AggregateId == aggregateId, cancellationToken);

    return _mapper.Map<Email>(email);
  }

  /// <summary>
  /// Retrieves a list of email addresses using the specified filters, sorting and paging arguments.
  /// </summary>
  /// <param name="isArchived">The value filtering email addresses on their archivation status.</param>
  /// <param name="isVerified">The value filtering email addresses on their verification status.</param>
  /// <param name="search">The text to search.</param>
  /// <param name="userId">The identifier of the user to filter by.</param>
  /// <param name="sort">The sort value.</param>
  /// <param name="isDescending">If true, the sort will be inverted.</param>
  /// <param name="skip">The number of email addresses to skip.</param>
  /// <param name="take">The number of email addresses to return.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The list of email addresses, or empty if none found.</returns>
  public async Task<PagedList<Email>> GetAsync(bool? isArchived, bool? isVerified, string? search, Guid? userId,
    EmailSort? sort, bool isDescending, int? skip, int? take, CancellationToken cancellationToken)
  {
    IQueryable<EmailEntity> query = _emails.AsNoTracking()
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

          query = query.Where(x => EF.Functions.ILike(x.Address, pattern)
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
        case EmailSort.Address:
          query = isDescending ? query.OrderByDescending(x => x.Address) : query.OrderBy(x => x.Address);
          break;
        case EmailSort.Label:
          query = isDescending ? query.OrderByDescending(x => x.Label) : query.OrderBy(x => x.Label);
          break;
        case EmailSort.UpdatedOn:
          query = isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn);
          break;
        case EmailSort.VerifiedOn:
          query = isDescending ? query.OrderByDescending(x => x.VerifiedOn) : query.OrderBy(x => x.VerifiedOn);
          break;
      }
    }

    query = query.ApplyPaging(skip, take);

    EmailEntity[] emails = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Email>(_mapper.Map<IEnumerable<Email>>(emails), total);
  }
}
