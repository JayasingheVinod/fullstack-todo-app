using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence.Configurations;

public sealed class TodoItemConfiguration
    : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.HasKey(todo => todo.Id);

        builder.Property(todo => todo.Title)
            .IsRequired()
            .HasMaxLength(TodoItem.MaxTitleLength);

        builder.Property(todo => todo.UserId)
            .IsRequired();

        builder.Property(todo => todo.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(todo => todo.UserId);
    }
}