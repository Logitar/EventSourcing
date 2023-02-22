using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// The model configuration used to configure the <see cref="ApiKeyEntity"/> class.
/// </summary>
internal class ApiKeyConfiguration : AggregateConfiguration<ApiKeyEntity>, IEntityTypeConfiguration<ApiKeyEntity>
{
  /// <summary>
  /// Configures the database model using the specified type builder.
  /// </summary>
  /// <param name="builder">The type builder.</param>
  public override void Configure(EntityTypeBuilder<ApiKeyEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.ApiKeyId);

    builder.HasOne(x => x.Realm).WithMany(x => x.ApiKeys).OnDelete(DeleteBehavior.Restrict);
    builder.HasMany(x => x.Roles).WithMany(x => x.ApiKeys);

    builder.HasIndex(x => x.Title);
    builder.HasIndex(x => x.ExpiresOn);

    builder.Property(x => x.Title).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CustomAttributes).HasColumnType("jsonb");
  }
}
