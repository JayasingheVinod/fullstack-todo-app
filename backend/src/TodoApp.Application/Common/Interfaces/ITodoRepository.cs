using TodoApp.Domain.Entities;

namespace TodoApp.Application.Common.Interfaces;

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoItem>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<TodoItem?> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        TodoItem todo,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        TodoItem todo,
        CancellationToken cancellationToken);
}