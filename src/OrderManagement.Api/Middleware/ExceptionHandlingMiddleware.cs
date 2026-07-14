using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var problemDetails = exception switch
        {
            ValidationException validationException =>
                CreateValidationProblemDetails(validationException),

            NotFoundException =>
                CreateProblemDetails(
                    StatusCodes.Status404NotFound,
                    "Recurso não encontrado",
                    exception.Message),

            ConflictException =>
                CreateProblemDetails(
                    StatusCodes.Status409Conflict,
                    "Conflito",
                    exception.Message),

            DomainException =>
                CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Regra de negócio inválida",
                    exception.Message),

            _ =>
                CreateProblemDetails(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno",
                    "Ocorreu um erro inesperado.")
        };

        if (problemDetails.Status >= 500)
        {
            _logger.LogError(
                exception,
                "Erro não tratado durante a requisição {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Erro tratado durante a requisição {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
        }

        context.Response.StatusCode =
            problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static ProblemDetails CreateProblemDetails(
        int status,
        string title,
        string detail)
    {
        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(
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

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Erro de validação",
            Detail = "Um ou mais campos são inválidos."
        };
    }
}