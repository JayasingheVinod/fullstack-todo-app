namespace TodoApp.Domain.Entities;

public sealed class TodoItem
{
    public const int MaxTitleLength = 200;

    private TodoItem()
    {
    }

    public TodoItem(string title, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Todo title cannot be empty.", nameof(title));
        }

        if (title.Length > MaxTitleLength)
        {
            throw new ArgumentException(
                $"Todo title cannot exceed {MaxTitleLength} characters.",
                nameof(title));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        Id = Guid.NewGuid();
        Title = title.Trim();
        UserId = userId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public Guid UserId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
}