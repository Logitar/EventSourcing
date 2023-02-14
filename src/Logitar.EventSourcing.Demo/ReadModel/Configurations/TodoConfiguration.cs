using Logitar.EventSourcing.Demo.ReadModel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.EventSourcing.Demo.ReadModel.Configurations;

internal class TodoConfiguration : AggregateConfiguration<TodoEntity>, IEntityTypeConfiguration<TodoEntity>
{
  public override void Configure(EntityTypeBuilder<TodoEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.TodoId);

    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.IsCompleted);

    builder.Property(x => x.Name).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.IsCompleted).HasDefaultValue(false);
  }
}
