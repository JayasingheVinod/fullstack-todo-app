using MediatR;

namespace TodoApp.Application.Todos.DeleteTodo;

public sealed record DeleteTodoCommand(Guid Id) : IRequest<Unit>;