using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Application.Todos.Common;

namespace TodoApp.Application.Todos.GetTodos;

public sealed class GetTodosQueryHandler
    : IRequestHandler<GetTodosQuery, IReadOnlyList<TodoDto>>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetTodosQueryHandler(
        ITodoRepository todoRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _todoRepository = todoRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TodoDto>> Handle(
        GetTodosQuery request,
        CancellationToken cancellationToken)
    {
        var todos = await _todoRepository.GetByUserIdAsync(
            _currentUserService.UserId,
            cancellationToken);

        return _mapper.Map<List<TodoDto>>(todos);
    }
}