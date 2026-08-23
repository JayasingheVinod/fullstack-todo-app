namespace TodoApp.Infrastructure.Authentication;

internal sealed record JwtToken(
    string Value,
    DateTime ExpiresAtUtc);