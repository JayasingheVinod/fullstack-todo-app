using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Repositories;

namespace TodoApp.Infrastructure;

public static class DependencyInjection
{
    private const string DatabaseName = "TodoAppDb";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddDbContext<TodoDbContext>(options =>
        {
            options.UseInMemoryDatabase(DatabaseName);
        });

        services.AddScoped<ITodoRepository, TodoRepository>();

        return services;
    }
}