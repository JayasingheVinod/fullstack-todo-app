using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Common;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Todos.CreateTodo;

public sealed class CreateTodoCommandHandler
    : IRequestHandler<CreateTodoCommand, TodoDto>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateTodoCommandHandler(
        ITodoRepository todoRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TodoDto> Handle(
        CreateTodoCommand request,
        CancellationToken cancellationToken)
    {
        var todo = new TodoItem(
            request.Title,
            _currentUserService.UserId);

        await _todoRepository.AddAsync(
            todo,
            cancellationToken);

        return _mapper.Map<TodoDto>(todo);
    }
}