 using System.Diagnostics;

namespace TalentShowCase.API.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;
            _logger.LogInformation(
                "Request {Method} {Path} completed in {ElapsedMilliseconds}ms with status code {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                elapsed,
                context.Response.StatusCode);
        }
    }
}