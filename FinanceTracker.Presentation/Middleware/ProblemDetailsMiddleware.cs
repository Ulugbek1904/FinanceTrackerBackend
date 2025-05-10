using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using FinanceTracker.Domain.Exceptions;

public class ProblemDetailsMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ProblemDetailsMiddleware> logger;

    public ProblemDetailsMiddleware(RequestDelegate next,
        ILogger<ProblemDetailsMiddleware> logger,
        ProblemDetailsFactory problemDetailsFactory)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await this.next(httpContext);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Exception caught in middleware");

            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        ProblemDetails problem;

        switch (exception)
        {
            case UnauthorizedAccessException ex:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                problem = new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = ex.Message,
                    Status = StatusCodes.Status401Unauthorized,
                    Type = "https://httpstatuses.com/401",
                    Instance = context.Request.Path
                };
                break;
            case ForbiddenAccessException ex:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                problem = new ProblemDetails
                {
                    Title = "Forbidden",
                    Detail = ex.Message,
                    Status = StatusCodes.Status403Forbidden,
                    Type = "https://httpstatuses.com/403",
                    Instance = context.Request.Path
                };
                break;

            case AppException appEx:
                context.Response.StatusCode = appEx.StatusCode;
                problem = new ProblemDetails
                {
                    Title = "Error was occurred",
                    Detail = appEx.Message,
                    Status = appEx.StatusCode,
                    Type = appEx.Type,
                    Instance = context.Request.Path
                };
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                problem = new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "Unexpected error occurred.",
                    Status = 500,
                    Type = "https://httpstatuses.com/500",
                    Instance = context.Request.Path
                };
                break;
        
        }

        var json = JsonSerializer.Serialize(problem);
        await context.Response.WriteAsync(json);
    }

}
