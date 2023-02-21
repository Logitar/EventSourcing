using AutoMapper;
using Logitar.Identity.Contacts;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// The profile used to configure mapping of phone numbers.
/// </summary>
internal class PhoneProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="PhoneProfile"/> class.
  /// </summary>
  public PhoneProfile()
  {
    CreateMap<PhoneEntity, Phone>()
      .IncludeBase<ContactEntity, Contact>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.ToGuid));
  }
}
