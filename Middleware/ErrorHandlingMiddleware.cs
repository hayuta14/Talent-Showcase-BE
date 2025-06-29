using System.Net;
using System.Text.Json;
using TalentShowCase.API.DTOs.Common;

namespace TalentShowCase.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {   
        context.Response.ContentType = "application/json";
        Console.WriteLine("Exception: " + exception.Message);
        var (response, statusCode) = HandleException(exception, context);
        context.Response.StatusCode = (int)statusCode;
        
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
        Console.WriteLine("Response: " + json);
        await context.Response.WriteAsync(json);
    }

    private static (ApiResponse<string> Response, HttpStatusCode StatusCode) HandleException(Exception exception, HttpContext context)
    {
        var isDevelopment = context.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>()?.IsDevelopment() == true;
        var errors = isDevelopment ? new List<string> { exception.Message, exception.StackTrace ?? string.Empty } : null;

        return exception switch
        {
            UnauthorizedAccessException => (ApiResponse<string>.Fail("Unauthorized access", 401, new List<string> { exception.Message }), HttpStatusCode.Unauthorized),
            InvalidOperationException => (ApiResponse<string>.Fail(exception.Message, 400, new List<string> { exception.Message }), HttpStatusCode.BadRequest),
            KeyNotFoundException => (ApiResponse<string>.Fail(exception.Message, 404, new List<string> { exception.Message }), HttpStatusCode.NotFound),
            _ => (ApiResponse<string>.Fail("An unexpected error occurred", 500, new List<string> { exception.Message }), HttpStatusCode.InternalServerError)
        };
    }
} 