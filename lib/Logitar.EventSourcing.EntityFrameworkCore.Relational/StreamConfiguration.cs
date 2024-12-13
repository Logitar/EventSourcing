using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.EntityFrameworkCore.Relational;

/// <summary>
/// The configuration for the database entity of type <see cref="StreamEntity"/>.
/// </summary>
public sealed class StreamConfiguration : IEntityTypeConfiguration<StreamEntity>
{
  /// <summary>
  /// Configures the database entity of type <see cref="StreamEntity" />.
  /// </summary>
  /// <param name="builder">The builder to be used to configure the database entity type.</param>
  public void Configure(EntityTypeBuilder<StreamEntity> builder)
  {
    builder.ToTable(EventDb.Streams.Table.Table!, EventDb.Streams.Table.Schema);
    builder.HasKey(x => x.StreamId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Type);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);
    builder.HasIndex(x => x.IsDeleted);

    builder.Property(x => x.Id).IsRequired().HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.Type).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);
  }
}
