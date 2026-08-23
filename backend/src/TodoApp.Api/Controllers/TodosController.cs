using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.Contracts;
using TodoApp.Application.Todos.Common;
using TodoApp.Application.Todos.CreateTodo;
using TodoApp.Application.Todos.DeleteTodo;
using TodoApp.Application.Todos.GetTodos;

namespace TodoApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/todos")]
public sealed class TodosController : ControllerBase
{
    private readonly ISender _sender;

    public TodosController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TodoDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<TodoDto>>> GetTodos(
        CancellationToken cancellationToken)
    {
        var todos = await _sender.Send(
            new GetTodosQuery(),
            cancellationToken);

        return Ok(todos);
    }

    [HttpPost]
    [ProducesResponseType<TodoDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TodoDto>> CreateTodo(
        CreateTodoRequest request,
        CancellationToken cancellationToken)
    {
        var todo = await _sender.Send(
            new CreateTodoCommand(request.Title),
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            todo);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTodo(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new DeleteTodoCommand(id),
            cancellationToken);

        return NoContent();
    }
}