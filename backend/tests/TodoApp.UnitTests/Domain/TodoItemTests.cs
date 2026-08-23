using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Domain;

public sealed class TodoItemTests
{
    [Fact]
    public void Constructor_WithValidValues_CreatesTodo()
    {
        var userId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        var todo = new TodoItem(
            "Prepare interview",
            userId);

        var afterCreation = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, todo.Id);
        Assert.Equal("Prepare interview", todo.Title);
        Assert.Equal(userId, todo.UserId);

        Assert.InRange(
            todo.CreatedAtUtc,
            beforeCreation,
            afterCreation);
    }

    [Fact]
    public void Constructor_TrimsTitle()
    {
        var todo = new TodoItem(
            "   Buy groceries   ",
            Guid.NewGuid());

        Assert.Equal(
            "Buy groceries",
            todo.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("     ")]
    public void Constructor_WithEmptyTitle_ThrowsArgumentException(
        string title)
    {
        var action = () =>
            new TodoItem(
                title,
                Guid.NewGuid());

        var exception =
            Assert.Throws<ArgumentException>(action);

        Assert.Equal(
            "title",
            exception.ParamName);
    }

    [Fact]
    public void Constructor_WithTitleOverMaximumLength_ThrowsArgumentException()
    {
        var title =
            new string(
                'A',
                TodoItem.MaxTitleLength + 1);

        var action = () =>
            new TodoItem(
                title,
                Guid.NewGuid());

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Constructor_WithMaximumLengthTitleAndWhitespace_IsValid()
    {
        var title =
            $"   {new string('A', TodoItem.MaxTitleLength)}   ";

        var todo = new TodoItem(
            title,
            Guid.NewGuid());

        Assert.Equal(
            TodoItem.MaxTitleLength,
            todo.Title.Length);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ThrowsArgumentException()
    {
        var action = () =>
            new TodoItem(
                "Prepare interview",
                Guid.Empty);

        var exception =
            Assert.Throws<ArgumentException>(action);

        Assert.Equal(
            "userId",
            exception.ParamName);
    }
}