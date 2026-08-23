using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoApp.Api.IntegrationTests.Infrastructure;

namespace TodoApp.Api.IntegrationTests;

public sealed class TodoApiTests
{
    [Fact]
    public async Task GetTodos_WithoutToken_ReturnsUnauthorized()
    {
        using var factory =
            new TodoAppFactory();

        using var client =
            factory.CreateClient();

        var response =
            await client.GetAsync(
                "/api/todos");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        using var factory =
            new TodoAppFactory();

        using var client =
            factory.CreateClient();

        var response =
            await client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    Email = "demo@todo.local",
                    Password = "wrong-password"
                });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_WithBlankTitle_ReturnsBadRequest()
    {
        using var factory =
            new TodoAppFactory();

        using var client =
            factory.CreateClient();

        await AuthenticateAsync(client);

        var response =
            await client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    Title = ""
                });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task TodoLifecycle_WithValidToken_CanCreateReadAndDelete()
    {
        using var factory =
            new TodoAppFactory();

        using var client =
            factory.CreateClient();

        await AuthenticateAsync(client);

        var createResponse =
            await client.PostAsJsonAsync(
                "/api/todos",
                new
                {
                    Title = "Prepare technical interview"
                });

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var createdTodo =
            await createResponse.Content
                .ReadFromJsonAsync<TodoResponse>();

        Assert.NotNull(createdTodo);

        Assert.NotEqual(
            Guid.Empty,
            createdTodo.Id);

        Assert.Equal(
            "Prepare technical interview",
            createdTodo.Title);

        var todos =
            await client.GetFromJsonAsync<List<TodoResponse>>(
                "/api/todos");

        Assert.NotNull(todos);

        var todo =
            Assert.Single(todos);

        Assert.Equal(
            createdTodo.Id,
            todo.Id);

        var deleteResponse =
            await client.DeleteAsync(
                $"/api/todos/{createdTodo.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        var todosAfterDelete =
            await client.GetFromJsonAsync<List<TodoResponse>>(
                "/api/todos");

        Assert.NotNull(todosAfterDelete);
        Assert.Empty(todosAfterDelete);
    }

    [Fact]
    public async Task DeleteTodo_WhenTodoDoesNotExist_ReturnsNotFound()
    {
        using var factory =
            new TodoAppFactory();

        using var client =
            factory.CreateClient();

        await AuthenticateAsync(client);

        var response =
            await client.DeleteAsync(
                $"/api/todos/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    private static async Task AuthenticateAsync(
        HttpClient client)
    {
        var response =
            await client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    Email = "demo@todo.local",
                    Password = "Todo123!"
                });

        response.EnsureSuccessStatusCode();

        var login =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(login);
        Assert.False(
            string.IsNullOrWhiteSpace(
                login.AccessToken));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);
    }

    private sealed record LoginResponse(
        string AccessToken,
        DateTime ExpiresAtUtc,
        string Email);

    private sealed record TodoResponse(
        Guid Id,
        string Title,
        DateTime CreatedAtUtc);
}