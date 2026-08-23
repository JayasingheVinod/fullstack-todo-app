namespace TodoApp.Infrastructure.Authentication;

internal interface IJwtTokenGenerator
{
    JwtToken Generate(
        Guid userId,
        string email);
}