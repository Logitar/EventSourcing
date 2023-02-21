using AutoMapper;
using Logitar.Identity.Contacts;
using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Profiles;

/// <summary>
/// The profile used to configure mapping of email addresses.
/// </summary>
internal class EmailProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EmailProfile"/> class.
  /// </summary>
  public EmailProfile()
  {
    CreateMap<EmailEntity, Email>()
      .IncludeBase<ContactEntity, Contact>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.ToGuid));
  }
}
