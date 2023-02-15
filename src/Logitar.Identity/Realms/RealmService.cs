using Logitar.Identity.Realms.Commands;
using Logitar.Identity.Realms.Queries;

namespace Logitar.Identity.Realms;

/// <summary>
/// Implements methods to manage realms in the identity system.
/// </summary>
internal class RealmService : IRealmService
{
  /// <summary>
  /// The request pipeline.
  /// </summary>
  private readonly IRequestPipeline _requestPipeline;

  /// <summary>
  /// Initializes a new instance of the <see cref="RealmService"/> class using the specified arguments.
  /// </summary>
  /// <param name="requestPipeline">The request pipeline.</param>
  public RealmService(IRequestPipeline requestPipeline)
  {
    _requestPipeline = requestPipeline;
  }

  /// <summary>
  /// Creates a new realm.
  /// </summary>
  /// <param name="input">The input creation arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The newly created realm.</returns>
  public async Task<Realm> CreateAsync(CreateRealmInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new CreateRealmCommand(input), cancellationToken);
  }

  /// <summary>
  /// Deletes a realm.
  /// </summary>
  /// <param name="id">The identifier of the realm.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted realm.</returns>
  public async Task<Realm> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new DeleteRealmCommand(id), cancellationToken);
  }

  /// <summary>
  /// Retrieves a realm by the specified unique values.
  /// </summary>
  /// <param name="id">The identifier of the realm.</param>
  /// <param name="uniqueName">The unique name of the realm.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The realm, or null if not found.</returns>
  public async Task<Realm?> GetAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetRealmQuery(id, uniqueName), cancellationToken);
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
  /// <returns>The realm list, or empty if none found.</returns>
  public async Task<PagedList<Realm>> GetAsync(string? search, RealmSort? sort, bool isDescending,
    int? skip, int? take, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new GetRealmsQuery(search, sort, isDescending, skip, take), cancellationToken);
  }

  /// <summary>
  /// Updates a realm.
  /// </summary>
  /// <param name="id">The identifier of the realm.</param>
  /// <param name="input">The input update arguments.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated realm.</returns>
  public async Task<Realm> UpdateAsync(Guid id, UpdateRealmInput input, CancellationToken cancellationToken)
  {
    return await _requestPipeline.ExecuteAsync(new UpdateRealmCommand(id, input), cancellationToken);
  }
}
