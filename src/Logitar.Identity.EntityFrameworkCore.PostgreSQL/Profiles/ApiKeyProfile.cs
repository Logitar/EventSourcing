using AutoMapper;
using Logitar.Identity.ApiKeys;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// The profile used to configure mapping of API keys.
/// </summary>
internal class ApiKeyProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ApiKeyProfile"/> class.
  /// </summary>
  public ApiKeyProfile()
  {
    CreateMap<ApiKeyEntity, ApiKey>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.ToGuid))
      .ForMember(x => x.CustomAttributes, x => x.MapFrom(MappingHelper.GetCustomAttributes));
  }
}
