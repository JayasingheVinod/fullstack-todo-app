using TodoApp.Api.Services;
using TodoApp.Application;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet(
    "/",
    () => Results.Ok("Todo API is running"));

app.Run();