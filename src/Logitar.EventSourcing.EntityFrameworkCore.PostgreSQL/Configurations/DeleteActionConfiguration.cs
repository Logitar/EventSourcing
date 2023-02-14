using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// The entity configuration used to configure the <see cref="DeleteActionEntity"/> database model.
/// </summary>
public class DeleteActionConfiguration : EnumEntityConfiguration<DeleteActionEntity, int>, IEntityTypeConfiguration<DeleteActionEntity>
{
  /// <summary>
  /// Configures the database model using the specified type builder.
  /// </summary>
  /// <param name="builder">The type builder</param>
  public override void Configure(EntityTypeBuilder<DeleteActionEntity> builder)
  {
    base.Configure(builder);

    builder.HasData(DeleteActionEntity.GetData());
  }
}
