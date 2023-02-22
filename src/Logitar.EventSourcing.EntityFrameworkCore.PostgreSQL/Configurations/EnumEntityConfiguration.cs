using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// The entity configuration used to configure the <see cref="EnumEntity{T}"/> database model.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public abstract class EnumEntityConfiguration<TEntity, TKey> where TEntity : EnumEntity<TKey>
{
  /// <summary>
  /// Configures the database model using the specified type builder.
  /// </summary>
  /// <param name="builder">The type builder.</param>
  public virtual void Configure(EntityTypeBuilder<TEntity> builder)
  {
    builder.HasKey(x => x.Value);

    builder.HasIndex(x => x.Name).IsUnique();

    builder.Property(x => x.Value).ValueGeneratedNever();
    builder.Property(x => x.Name).HasMaxLength(byte.MaxValue);
  }
}
