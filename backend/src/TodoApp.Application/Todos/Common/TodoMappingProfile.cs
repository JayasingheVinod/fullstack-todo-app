using AutoMapper;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Todos.Common;

public sealed class TodoMappingProfile : Profile
{
    public TodoMappingProfile()
    {
        CreateMap<TodoItem, TodoDto>();
    }
}