using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using TodoApp.Application.Todos.Common;

namespace TodoApp.UnitTests.TestDoubles;

internal static class TestMapperFactory
{
    public static IMapper Create()
    {
        var configuration =
            new MapperConfiguration(
                config =>
                {
                    config.AddProfile<TodoMappingProfile>();
                },
                NullLoggerFactory.Instance);

        configuration.AssertConfigurationIsValid();

        return configuration.CreateMapper();
    }
}