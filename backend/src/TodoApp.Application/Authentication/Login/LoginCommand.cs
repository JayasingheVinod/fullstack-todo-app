using MediatR;

namespace TodoApp.Application.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<LoginResponse>;