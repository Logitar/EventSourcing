using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
  public void Configure(EntityTypeBuilder<EventEntity> builder)
  {
    builder.HasKey(x => x.EventId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.ActorId);
    builder.HasIndex(x => x.OccurredOn);
    builder.HasIndex(x => x.DeleteAction);
    builder.HasIndex(x => new { x.AggregateType, x.AggregateId });
    builder.HasIndex(x => x.AggregateId);
    builder.HasIndex(x => x.EventType);

    builder.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
    builder.Property(x => x.Version).HasDefaultValue((long)0);
    builder.Property(x => x.ActorId).HasMaxLength(byte.MaxValue).HasDefaultValue("SYSTEM");
    builder.Property(x => x.OccurredOn).HasDefaultValueSql("now()");
    builder.Property(x => x.DeleteAction).HasDefaultValue(default(DeleteAction));
    builder.Property(x => x.AggregateType).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.AggregateId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EventType).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.EventData).HasColumnType("jsonb");
  }
}
