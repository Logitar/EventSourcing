using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// The configuration for the entity of type <see cref="EventEntity"/>.
/// </summary>
public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
  /// <summary>
  /// Configures the entity of type <see cref="EventEntity" />.
  /// </summary>
  /// <param name="builder">The builder to be used to configure the entity type.</param>
  public void Configure(EntityTypeBuilder<EventEntity> builder)
  {
    builder.ToTable(nameof(EventContext.Events));
    builder.HasKey(x => x.EventId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.ActorId);
    builder.HasIndex(x => x.IsDeleted);
    builder.HasIndex(x => x.OccurredOn);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.AggregateId);
    builder.HasIndex(x => x.EventType);
    builder.HasIndex(x => new { x.AggregateType, x.AggregateId });

    builder.Property(x => x.ActorId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AggregateType).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AggregateId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EventType).HasMaxLength(byte.MaxValue);
  }
}
