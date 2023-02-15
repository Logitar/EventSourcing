using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// The model configuration used to configure the <see cref="RealmEntity"/> class.
/// </summary>
internal class RealmConfiguration : AggregateConfiguration<RealmEntity>, IEntityTypeConfiguration<RealmEntity>
{
  /// <summary>
  /// Configures the database model using the specified type builder.
  /// </summary>
  /// <param name="builder">The type builder.</param>
  public override void Configure(EntityTypeBuilder<RealmEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.RealmId);

    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.UniqueNameNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DefaultLocale).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Url).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.RequireConfirmedAccount).HasDefaultValue(false);
    builder.Property(x => x.RequireUniqueEmail).HasDefaultValue(false);
    builder.Property(x => x.UsernameSettings).HasColumnType("jsonb");
    builder.Property(x => x.PasswordSettings).HasColumnType("jsonb");
    builder.Property(x => x.CustomAttributes).HasColumnType("jsonb");
    builder.Property(x => x.GoogleOAuth2Configuration).HasColumnType("jsonb");
  }
}
