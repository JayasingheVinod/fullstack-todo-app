using FluentValidation;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Todos.CreateTodo;

public sealed class CreateTodoCommandValidator
    : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .Must(HaveValidLength)
            .WithMessage(
                $"Todo title cannot exceed {TodoItem.MaxTitleLength} characters.");
    }

    private static bool HaveValidLength(string title)
    {
        return string.IsNullOrWhiteSpace(title) ||
               title.Trim().Length <= TodoItem.MaxTitleLength;
    }
}