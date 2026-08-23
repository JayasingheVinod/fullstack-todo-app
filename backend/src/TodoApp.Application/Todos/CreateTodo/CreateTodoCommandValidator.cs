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
            .MaximumLength(TodoItem.MaxTitleLength);
    }
}