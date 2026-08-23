using MediatR;
using TodoApp.Application.Todos.Common;

namespace TodoApp.Application.Todos.GetTodos;

public sealed record GetTodosQuery()
    : IRequest<IReadOnlyList<TodoDto>>;