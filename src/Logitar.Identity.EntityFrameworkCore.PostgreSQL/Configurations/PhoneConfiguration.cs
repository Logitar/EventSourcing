using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// The model configuration used to configure the <see cref="PhoneEntity"/> class.
/// </summary>
internal class PhoneConfiguration : ContactConfiguration<PhoneEntity>, IEntityTypeConfiguration<PhoneEntity>
{
  /// <summary>
  /// Configures the database model using the specified type builder.
  /// </summary>
  /// <param name="builder">The type builder.</param>
  public override void Configure(EntityTypeBuilder<PhoneEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.PhoneId);

    builder.HasOne(x => x.User).WithMany(x => x.Phones).OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(x => x.CountryCode);
    builder.HasIndex(x => x.Number);
    builder.HasIndex(x => x.Extension);

    builder.Property(x => x.CountryCode).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Number).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Extension).HasMaxLength(byte.MaxValue);
  }
}
