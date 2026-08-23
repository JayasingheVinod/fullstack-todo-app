using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repositories;

public sealed class TodoRepository : ITodoRepository
{
    private readonly TodoDbContext _dbContext;

    public TodoRepository(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TodoItem>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Todos
            .AsNoTracking()
            .Where(todo => todo.UserId == userId)
            .OrderByDescending(todo => todo.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoItem?> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Todos
            .FirstOrDefaultAsync(
                todo => todo.Id == id &&
                        todo.UserId == userId,
                cancellationToken);
    }

    public async Task AddAsync(
        TodoItem todo,
        CancellationToken cancellationToken)
    {
        await _dbContext.Todos.AddAsync(
            todo,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        TodoItem todo,
        CancellationToken cancellationToken)
    {
        _dbContext.Todos.Remove(todo);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}