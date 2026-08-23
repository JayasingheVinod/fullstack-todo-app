using TodoApp.Application.Todos.GetTodos;
using TodoApp.Domain.Entities;
using TodoApp.UnitTests.TestDoubles;

namespace TodoApp.UnitTests.Application.GetTodos;

public sealed class GetTodosQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyTodosForCurrentUser()
    {
        var currentUserId =
            Guid.NewGuid();

        var otherUserId =
            Guid.NewGuid();

        var repository =
            new FakeTodoRepository();

        repository.Items.Add(
            new TodoItem(
                "Current user todo",
                currentUserId));

        repository.Items.Add(
            new TodoItem(
                "Another user todo",
                otherUserId));

        var handler =
            new GetTodosQueryHandler(
                repository,
                new StubCurrentUserService(
                    currentUserId),
                TestMapperFactory.Create());

        var result =
            await handler.Handle(
                new GetTodosQuery(),
                CancellationToken.None);

        var todo =
            Assert.Single(result);

        Assert.Equal(
            "Current user todo",
            todo.Title);

        Assert.Equal(
            currentUserId,
            repository.LastRequestedUserId);
    }
}