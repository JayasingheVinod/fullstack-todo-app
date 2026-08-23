using Microsoft.AspNetCore.Identity;
using TodoApp.Application.Authentication.Login;
using TodoApp.Application.Common.Interfaces;

namespace TodoApp.Infrastructure.Authentication;

internal sealed class DemoUserAuthenticationService
    : IUserAuthenticationService
{
    private readonly IPasswordHasher<DemoUser> _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly DemoUser _demoUser;

    public DemoUserAuthenticationService(
        IPasswordHasher<DemoUser> passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;

        _demoUser = new DemoUser(
            DemoCredentials.UserId,
            DemoCredentials.Email);

        _demoUser.PasswordHash =
            _passwordHasher.HashPassword(
                _demoUser,
                DemoCredentials.Password);
    }

    public Task<LoginResponse?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!string.Equals(
                email.Trim(),
                _demoUser.Email,
                StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult<LoginResponse?>(null);
        }

        var verificationResult =
            _passwordHasher.VerifyHashedPassword(
                _demoUser,
                _demoUser.PasswordHash,
                password);

        if (verificationResult ==
            PasswordVerificationResult.Failed)
        {
            return Task.FromResult<LoginResponse?>(null);
        }

        var token = _tokenGenerator.Generate(
            _demoUser.Id,
            _demoUser.Email);

        var response = new LoginResponse(
            token.Value,
            token.ExpiresAtUtc,
            _demoUser.Email);

        return Task.FromResult<LoginResponse?>(
            response);
    }
}