using System.Security.Claims;
using TalentShowCase.API.Services;

namespace TalentShowCase.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtService _jwtService;

    public JwtMiddleware(RequestDelegate next, IJwtService jwtService)
    {
        _next = next;
        _jwtService = jwtService;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            var principal = _jwtService.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
            }
        }

        await _next(context);
    }
} 