using Logitar.EventSourcing.Demo.ReadModel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.Demo.ReadModel.Configurations;

internal abstract class AggregateConfiguration<T> where T : AggregateEntity
{
  public virtual void Configure(EntityTypeBuilder<T> builder)
  {
    builder.HasIndex(x => x.AggregateId).IsUnique();
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.AggregateId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Version).HasDefaultValue((long)0);
    builder.Property(x => x.CreatedById).HasMaxLength(byte.MaxValue).HasDefaultValue("SYSTEM");
    builder.Property(x => x.CreatedOn).HasDefaultValueSql("now()");
    builder.Property(x => x.UpdatedById).HasMaxLength(byte.MaxValue);
  }
}
