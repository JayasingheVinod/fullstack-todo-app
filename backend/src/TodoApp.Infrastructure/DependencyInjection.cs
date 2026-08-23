using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Infrastructure.Authentication;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Repositories;

namespace TodoApp.Infrastructure;

public static class DependencyInjection
{
    private const string DatabaseName = "TodoAppDb";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence();
        services.AddAuthentication(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services)
    {
        services.AddDbContext<TodoDbContext>(options =>
        {
            options.UseInMemoryDatabase(DatabaseName);
        });

        services.AddScoped<ITodoRepository, TodoRepository>();

        return services;
    }

    private static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = CreateJwtOptions(configuration);

        services.AddSingleton(jwtOptions);

        services.AddSingleton<
            IPasswordHasher<DemoUser>,
            PasswordHasher<DemoUser>>();

        services.AddSingleton<
            IJwtTokenGenerator,
            JwtTokenGenerator>();

        services.AddSingleton<
            IUserAuthenticationService,
            DemoUserAuthenticationService>();

        services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Convert.FromBase64String(
                                    jwtOptions.SigningKey)),

                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
            });

        services.AddAuthorization();

        return services;
    }

    private static JwtOptions CreateJwtOptions(
        IConfiguration configuration)
    {
        var issuer =
            configuration[$"{JwtOptions.SectionName}:Issuer"];

        var audience =
            configuration[$"{JwtOptions.SectionName}:Audience"];

        var expirationValue =
            configuration[
                $"{JwtOptions.SectionName}:ExpirationMinutes"];

        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException(
                "JWT issuer is not configured.");
        }

        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException(
                "JWT audience is not configured.");
        }

        if (!int.TryParse(
                expirationValue,
                out var expirationMinutes) ||
            expirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "JWT expiration is not configured correctly.");
        }

        return new JwtOptions
        {
            Issuer = issuer,
            Audience = audience,
            ExpirationMinutes = expirationMinutes,

            SigningKey = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(32))
        };
    }
}