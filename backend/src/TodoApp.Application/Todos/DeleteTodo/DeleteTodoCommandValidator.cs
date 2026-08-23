using FluentValidation;

namespace TodoApp.Application.Todos.DeleteTodo;

public sealed class DeleteTodoCommandValidator
    : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}