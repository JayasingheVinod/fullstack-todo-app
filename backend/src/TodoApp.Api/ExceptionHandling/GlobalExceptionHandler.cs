using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common.Exceptions;

namespace TodoApp.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = CreateProblemDetails(
            httpContext,
            exception);

        if (problemDetails.Status >=
            StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        httpContext.Response.StatusCode =
            problemDetails.Status ??
            StatusCodes.Status500InternalServerError;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        Exception exception)
    {
        return exception switch
        {
            ValidationException validationException =>
                CreateValidationProblemDetails(
                    httpContext,
                    validationException),

            NotFoundException =>
                CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status404NotFound,
                    "Resource not found",
                    exception.Message),

            UnauthorizedException =>
                CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    exception.Message),

            UnauthorizedAccessException =>
                CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Authentication is required."),

            _ =>
                CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred",
                    "An unexpected error occurred while processing the request.")
        };
    }

    private static ProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .Distinct()
                    .ToArray());

        var problemDetails = CreateProblemDetails(
            httpContext,
            StatusCodes.Status400BadRequest,
            "Validation failed",
            "One or more validation errors occurred.");

        problemDetails.Extensions["errors"] = errors;

        return problemDetails;
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int status,
        string title,
        string detail)
    {
        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
    }
}