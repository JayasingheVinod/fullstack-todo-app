using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Api.Services;

public sealed class CurrentUserService
    : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId =
                _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirst("sub")?
                    .Value;

            if (!Guid.TryParse(
                    userId,
                    out var parsedUserId))
            {
                throw new UnauthorizedAccessException(
                    "Authenticated user identifier is unavailable.");
            }

            return parsedUserId;
        }
    }
}