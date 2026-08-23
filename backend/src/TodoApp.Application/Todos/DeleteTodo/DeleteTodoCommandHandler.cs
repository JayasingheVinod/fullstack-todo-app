using MediatR;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Todos.DeleteTodo;

public sealed class DeleteTodoCommandHandler
    : IRequestHandler<DeleteTodoCommand, Unit>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTodoCommandHandler(
        ITodoRepository todoRepository,
        ICurrentUserService currentUserService)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(
        DeleteTodoCommand request,
        CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdAsync(
            request.Id,
            _currentUserService.UserId,
            cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException(
                nameof(TodoItem),
                request.Id);
        }

        await _todoRepository.DeleteAsync(
            todo,
            cancellationToken);

        return Unit.Value;
    }
}