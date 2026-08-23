using MediatR;
using TodoApp.Application.Todos.Common;

namespace TodoApp.Application.Todos.CreateTodo;

public sealed record CreateTodoCommand(string Title) : IRequest<TodoDto>;