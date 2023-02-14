using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Configurations;

public abstract class EnumEntityConfiguration<TEntity, TKey> where TEntity : EnumEntity<TKey>
{
  public virtual void Configure(EntityTypeBuilder<TEntity> builder)
  {
    builder.HasKey(x => x.Value);

    builder.HasIndex(x => x.Name).IsUnique();

    builder.Property(x => x.Value).ValueGeneratedNever();
    builder.Property(x => x.Name).HasMaxLength(byte.MaxValue);
  }
}
