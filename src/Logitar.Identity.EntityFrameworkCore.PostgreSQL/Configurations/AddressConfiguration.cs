using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// The model configuration used to configure the <see cref="AddressEntity"/> class.
/// </summary>
internal class AddressConfiguration : ContactConfiguration<AddressEntity>, IEntityTypeConfiguration<AddressEntity>
{
  /// <summary>
  /// Configures the database model using the specified type builder.
  /// </summary>
  /// <param name="builder">The type builder.</param>
  public override void Configure(EntityTypeBuilder<AddressEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.AddressId);

    builder.HasOne(x => x.User).WithMany(x => x.Addresses).OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(x => x.Line1);
    builder.HasIndex(x => x.Line2);
    builder.HasIndex(x => x.Locality);
    builder.HasIndex(x => x.PostalCode);
    builder.HasIndex(x => x.Country);
    builder.HasIndex(x => x.Region);

    builder.Property(x => x.Line1).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Line2).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Locality).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PostalCode).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Country).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Region).HasMaxLength(byte.MaxValue);
  }
}
