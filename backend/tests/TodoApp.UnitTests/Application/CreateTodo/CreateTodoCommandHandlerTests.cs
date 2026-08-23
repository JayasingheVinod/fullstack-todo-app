using TodoApp.Application.Todos.CreateTodo;
using TodoApp.UnitTests.TestDoubles;

namespace TodoApp.UnitTests.Application.CreateTodo;

public sealed class CreateTodoCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_AddsTodoForCurrentUser()
    {
        var userId = Guid.NewGuid();

        var repository =
            new FakeTodoRepository();

        var currentUser =
            new StubCurrentUserService(userId);

        var mapper =
            TestMapperFactory.Create();

        var handler =
            new CreateTodoCommandHandler(
                repository,
                currentUser,
                mapper);

        var command =
            new CreateTodoCommand(
                "Prepare interview");

        var result =
            await handler.Handle(
                command,
                CancellationToken.None);

        var storedTodo =
            Assert.Single(repository.Items);

        Assert.Equal(
            userId,
            storedTodo.UserId);

        Assert.Equal(
            "Prepare interview",
            storedTodo.Title);

        Assert.Equal(
            storedTodo.Id,
            result.Id);

        Assert.Equal(
            storedTodo.Title,
            result.Title);
    }

    [Fact]
    public async Task Handle_WithWhitespaceAroundTitle_StoresNormalizedTitle()
    {
        var repository =
            new FakeTodoRepository();

        var handler =
            new CreateTodoCommandHandler(
                repository,
                new StubCurrentUserService(
                    Guid.NewGuid()),
                TestMapperFactory.Create());

        var command =
            new CreateTodoCommand(
                "   Buy groceries   ");

        var result =
            await handler.Handle(
                command,
                CancellationToken.None);

        Assert.Equal(
            "Buy groceries",
            result.Title);
    }
}