using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Configurations;

public class DeleteActionConfiguration : EnumEntityConfiguration<DeleteActionEntity, int>, IEntityTypeConfiguration<DeleteActionEntity>
{
  public override void Configure(EntityTypeBuilder<DeleteActionEntity> builder)
  {
    base.Configure(builder);

    builder.HasData(DeleteActionEntity.GetData());
  }
}
