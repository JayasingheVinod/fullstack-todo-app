namespace TodoApp.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
}