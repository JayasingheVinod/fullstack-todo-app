namespace TodoApp.Infrastructure.Authentication;

internal sealed class DemoUser
{
    public DemoUser(
        Guid id,
        string email)
    {
        Id = id;
        Email = email;
    }

    public Guid Id { get; }

    public string Email { get; }

    public string PasswordHash { get; set; } = string.Empty;
}