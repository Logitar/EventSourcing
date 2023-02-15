using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// The model configuration used to configure the <see cref="RoleEntity"/> class.
/// </summary>
internal class RoleConfiguration : AggregateConfiguration<RoleEntity>, IEntityTypeConfiguration<RoleEntity>
{
  /// <summary>
  /// Configures the database model using the specified type builder.
  /// </summary>
  /// <param name="builder">The type builder.</param>
  public override void Configure(EntityTypeBuilder<RoleEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.RoleId);

    builder.HasOne(x => x.Realm).WithMany(x => x.Roles).OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CustomAttributes).HasColumnType("jsonb");
  }
}
