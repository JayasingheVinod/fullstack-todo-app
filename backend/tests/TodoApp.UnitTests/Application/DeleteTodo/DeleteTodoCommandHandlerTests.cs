using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Todos.DeleteTodo;
using TodoApp.Domain.Entities;
using TodoApp.UnitTests.TestDoubles;

namespace TodoApp.UnitTests.Application.DeleteTodo;

public sealed class DeleteTodoCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenTodoExists_DeletesTodo()
    {
        var userId =
            Guid.NewGuid();

        var todo =
            new TodoItem(
                "Delete me",
                userId);

        var repository =
            new FakeTodoRepository();

        repository.Items.Add(todo);

        var handler =
            new DeleteTodoCommandHandler(
                repository,
                new StubCurrentUserService(userId));

        await handler.Handle(
            new DeleteTodoCommand(todo.Id),
            CancellationToken.None);

        Assert.Empty(repository.Items);
    }

    [Fact]
    public async Task Handle_WhenTodoDoesNotExist_ThrowsNotFoundException()
    {
        var handler =
            new DeleteTodoCommandHandler(
                new FakeTodoRepository(),
                new StubCurrentUserService(
                    Guid.NewGuid()));

        var command =
            new DeleteTodoCommand(
                Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(
                command,
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenTodoBelongsToAnotherUser_ThrowsNotFoundException()
    {
        var ownerId =
            Guid.NewGuid();

        var otherUserId =
            Guid.NewGuid();

        var todo =
            new TodoItem(
                "Private todo",
                ownerId);

        var repository =
            new FakeTodoRepository();

        repository.Items.Add(todo);

        var handler =
            new DeleteTodoCommandHandler(
                repository,
                new StubCurrentUserService(
                    otherUserId));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(
                new DeleteTodoCommand(todo.Id),
                CancellationToken.None));

        Assert.Single(repository.Items);
    }
}