using TodoApp.Application.Todos.CreateTodo;
using TodoApp.Domain.Entities;

namespace TodoApp.UnitTests.Application.CreateTodo;

public sealed class CreateTodoCommandValidatorTests
{
    private readonly CreateTodoCommandValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidTitle_IsValid()
    {
        var command =
            new CreateTodoCommand(
                "Prepare technical interview");

        var result =
            await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithEmptyTitle_IsInvalid()
    {
        var command =
            new CreateTodoCommand("");

        var result =
            await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);

        Assert.Contains(
            result.Errors,
            error => error.PropertyName == nameof(command.Title));
    }

    [Fact]
    public async Task Validate_WithOverMaximumLengthTitle_IsInvalid()
    {
        var command =
            new CreateTodoCommand(
                new string(
                    'A',
                    TodoItem.MaxTitleLength + 1));

        var result =
            await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Validate_WithMaximumLengthTitleAndWhitespace_IsValid()
    {
        var title =
            $"   {new string('A', TodoItem.MaxTitleLength)}   ";

        var command =
            new CreateTodoCommand(title);

        var result =
            await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }
}