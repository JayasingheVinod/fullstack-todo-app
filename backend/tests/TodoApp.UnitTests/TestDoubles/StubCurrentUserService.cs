using TodoApp.Application.Common.Interfaces;

namespace TodoApp.UnitTests.TestDoubles;

internal sealed class StubCurrentUserService
    : ICurrentUserService
{
    public StubCurrentUserService(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}