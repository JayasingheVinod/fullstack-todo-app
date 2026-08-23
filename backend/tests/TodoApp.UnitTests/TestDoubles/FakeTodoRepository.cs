using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.TestDoubles;

internal sealed class FakeTodoRepository
    : ITodoRepository
{
    public List<TodoItem> Items { get; } = [];

    public Guid? LastRequestedUserId { get; private set; }

    public Task<IReadOnlyList<TodoItem>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        LastRequestedUserId = userId;

        IReadOnlyList<TodoItem> todos = Items
            .Where(todo => todo.UserId == userId)
            .OrderByDescending(todo => todo.CreatedAtUtc)
            .ToList();

        return Task.FromResult(todos);
    }

    public Task<TodoItem?> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        LastRequestedUserId = userId;

        var todo = Items.FirstOrDefault(
            todo =>
                todo.Id == id &&
                todo.UserId == userId);

        return Task.FromResult(todo);
    }

    public Task AddAsync(
        TodoItem todo,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Items.Add(todo);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        TodoItem todo,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Items.Remove(todo);

        return Task.CompletedTask;
    }
}