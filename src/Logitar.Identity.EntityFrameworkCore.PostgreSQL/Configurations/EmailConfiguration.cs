using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// The model configuration used to configure the <see cref="EmailEntity"/> class.
/// </summary>
internal class EmailConfiguration : ContactConfiguration<EmailEntity>, IEntityTypeConfiguration<EmailEntity>
{
  /// <summary>
  /// Configures the database model using the specified type builder.
  /// </summary>
  /// <param name="builder">The type builder.</param>
  public override void Configure(EntityTypeBuilder<EmailEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.EmailId);

    builder.HasOne(x => x.User).WithMany(x => x.Emails).OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(x => x.Address);

    builder.Property(x => x.Address).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.IsLogin).HasDefaultValue(false);
  }
}
