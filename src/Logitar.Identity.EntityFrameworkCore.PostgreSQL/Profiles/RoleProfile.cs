using AutoMapper;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Logitar.Identity.Roles;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// The profile used to configure mapping of roles.
/// </summary>
internal class RoleProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RoleProfile"/> class.
  /// </summary>
  public RoleProfile()
  {
    CreateMap<RoleEntity, Role>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.ToGuid))
      .ForMember(x => x.CustomAttributes, x => x.MapFrom(MappingHelper.GetCustomAttributes));
  }
}
