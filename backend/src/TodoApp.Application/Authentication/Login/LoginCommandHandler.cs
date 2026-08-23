using MediatR;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Application.Authentication.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserAuthenticationService _authenticationService;

    public LoginCommandHandler(
        IUserAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticationService.AuthenticateAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (result is null)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        return result;
    }
}