using AutoMapper;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// The profile used to configure mapping of aggregates.
/// </summary>
internal class AggregateProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AggregateProfile"/> class.
  /// </summary>
  public AggregateProfile()
  {
    CreateMap<AggregateEntity, Aggregate>()
      .ForMember(x => x.UpdatedOn, x => x.MapFrom(y => y.UpdatedOn ?? y.CreatedOn));
  }
}
