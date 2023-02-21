using AutoMapper;
using Logitar.Identity.Contacts;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// The profile used to configure mapping of postal addresses.
/// </summary>
internal class AddressProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AddressProfile"/> class.
  /// </summary>
  public AddressProfile()
  {
    CreateMap<AddressEntity, Address>()
      .IncludeBase<ContactEntity, Contact>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.ToGuid));
  }
}
