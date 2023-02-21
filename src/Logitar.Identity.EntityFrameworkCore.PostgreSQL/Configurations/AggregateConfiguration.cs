using Logitar.Identity.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Identity.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// The model configuration used to configure classes inheriting the <see cref="AggregateEntity"/> class.
/// </summary>
/// <typeparam name="T">The type of the inheriting class.</typeparam>
public abstract class AggregateConfiguration<T> where T : AggregateEntity
{
  /// <summary>
  /// Configures the database model using the specified type builder.
  /// </summary>
  /// <param name="builder">The type builder.</param>
  public virtual void Configure(EntityTypeBuilder<T> builder)
  {
    builder.HasIndex(x => x.AggregateId).IsUnique();
    builder.HasIndex(x => x.CreatedById);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedById);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.AggregateId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Version).HasDefaultValue((long)0);
    builder.Property(x => x.CreatedById).HasMaxLength(byte.MaxValue).HasDefaultValue("SYSTEM");
    builder.Property(x => x.CreatedOn).HasDefaultValueSql("now()");
    builder.Property(x => x.UpdatedById).HasMaxLength(byte.MaxValue);
  }
}
