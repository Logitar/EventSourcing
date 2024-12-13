using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// The configuration for the database entity of type <see cref="EventEntity"/>.
/// </summary>
public sealed class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
  /// <summary>
  /// Configures the database entity of type <see cref="EventEntity" />.
  /// </summary>
  /// <param name="builder">The builder to be used to configure the database entity type.</param>
  public void Configure(EntityTypeBuilder<EventEntity> builder)
  {
    builder.ToTable(EventDb.Events.Table.Table!, EventDb.Events.Table.Schema);
    builder.HasKey(x => x.EventId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => new { x.StreamId, x.Version }).IsUnique();
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.ActorId);
    builder.HasIndex(x => x.OccurredOn);
    builder.HasIndex(x => x.IsDeleted);
    builder.HasIndex(x => x.TypeName);

    builder.Property(x => x.Id).IsRequired().HasMaxLength(EventId.MaximumLength);
    builder.Property(x => x.ActorId).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.TypeName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.NamespacedType).HasMaxLength(byte.MaxValue);

    builder.HasOne(x => x.Stream).WithMany(x => x.Events)
      .HasPrincipalKey(x => x.StreamId).HasForeignKey(x => x.StreamId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
