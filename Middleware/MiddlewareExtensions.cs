namespace TalentShowCase.API.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }

    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
} 