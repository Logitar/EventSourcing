using AutoMapper;
using Logitar.Identity.Contacts;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// The profile used to configure mapping of contact informations.
/// </summary>
internal class ContactProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ContactProfile"/> class.
  /// </summary>
  public ContactProfile()
  {
    CreateMap<ContactEntity, Contact>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.CustomAttributes, x => x.MapFrom(MappingHelper.GetCustomAttributes));
  }
}
