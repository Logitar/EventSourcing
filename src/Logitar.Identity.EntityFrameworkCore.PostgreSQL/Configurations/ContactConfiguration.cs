using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// The model configuration used to configure classes inheriting the <see cref="ContactEntity"/> class.
/// </summary>
/// <typeparam name="T">The type of the inheriting class.</typeparam>
internal abstract class ContactConfiguration<T> : AggregateConfiguration<T> where T : ContactEntity
{
  /// <summary>
  /// Configures the database model using the specified type builder.
  /// </summary>
  /// <param name="builder">The type builder.</param>
  public override void Configure(EntityTypeBuilder<T> builder)
  {
    base.Configure(builder);

    builder.HasIndex(x => x.ArchivedById);
    builder.HasIndex(x => x.IsArchived);
    builder.HasIndex(x => x.VerifiedById);
    builder.HasIndex(x => x.VerifiedOn);
    builder.HasIndex(x => x.IsVerified);
    builder.HasIndex(x => x.Label);
    builder.HasIndex(x => new { x.UserId, x.IsDefault }).IsUnique().HasFilter(@"""IsDefault"" = true");

    builder.Property(x => x.ArchivedById).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.IsArchived).HasDefaultValue(false);
    builder.Property(x => x.IsDefault).HasDefaultValue(false);
    builder.Property(x => x.VerifiedById).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.IsVerified).HasDefaultValue(false);
    builder.Property(x => x.Label).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CustomAttributes).HasColumnType("jsonb");
  }
}
