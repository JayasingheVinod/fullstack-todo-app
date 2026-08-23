using TodoApp.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

var app = builder.Build();

app.MapGet("/", () => Results.Ok("Todo API is running"));

app.Run();