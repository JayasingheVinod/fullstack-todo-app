namespace TodoApp.Application.Todos.Common;

public sealed class TodoDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public DateTime CreatedAtUtc { get; init; }
}