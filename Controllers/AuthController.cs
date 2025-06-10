using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentShowCase.API.Data;
using TalentShowCase.API.Models.Auth;
using TalentShowCase.API.Services;
using TalentShowCase.API.DTOs.Auth;
using TalentShowCase.API.DTOs.Common;

namespace TalentShowCase.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(
        ApplicationDbContext context,
        IJwtService jwtService,
        ILogger<AuthController> logger,
        IAuthService authService)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterRequestDto request)
    {
        var (success, message) = await _authService.RegisterAsync(request);
        if (!success)
        {
            throw new InvalidOperationException(message);
        }

        return Ok(ApiResponse<string>.Succeed(message));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TokenDto>>> Login([FromBody] LoginRequestDto request)
    {
        var (success, tokens, message) = await _authService.LoginAsync(request);
        if (!success)
        {
            Console.WriteLine(message);
            throw new UnauthorizedAccessException(message);
        }

        return Ok(ApiResponse<TokenDto>.Succeed(tokens!));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TokenDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var (success, tokens, message) = await _authService.RefreshTokenAsync(request.RefreshToken);
        if (!success)
        {
            throw new UnauthorizedAccessException(message);
        }

        return Ok(ApiResponse<TokenDto>.Succeed(tokens!));
    }

    [HttpPost("revoke-token")]      
    [Authorize]
    public async Task<ActionResult<ApiResponse<string>>> RevokeToken([FromBody] RevokeTokenRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("Invalid user");
        }

        var (success, message) = await _authService.RevokeTokenAsync(request.RefreshToken, userId);
        if (!success)
        {
            throw new InvalidOperationException(message);
        }

        return Ok(ApiResponse<string>.Succeed(message));
    }
} 