using TodoApp.Application.Authentication.Login;

namespace TodoApp.Application.Common.Interfaces;

public interface IUserAuthenticationService
{
    Task<LoginResponse?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken);
}