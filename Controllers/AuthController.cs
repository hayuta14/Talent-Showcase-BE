using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalentShowCase.API.Data;
using TalentShowCase.API.Models;
using TalentShowCase.API.Services;
using TalentShowCase.API.DTOs.Auth;
using TalentShowCase.API.DTOs.Common;

namespace TalentShowCase.API.Controllers;

/// <summary>
/// Authentication controller for user registration, login, and token management
/// </summary>
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

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">User registration information</param>
    /// <returns>Success message if registration is successful</returns>
    /// <response code="200">Registration successful</response>
    /// <response code="400">Invalid registration data</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 400)]
    public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterRequestDto request)
    {
        var (success, message) = await _authService.RegisterAsync(request);
        if (!success)
        {
            throw new InvalidOperationException(message);
        }

        return Ok(ApiResponse<string>.Succeed(message));
    }

    /// <summary>
    /// Authenticate user and return access token
    /// </summary>
    /// <param name="request">User login credentials</param>
    /// <returns>Access token and refresh token</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TokenDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 401)]
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

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New access token and refresh token</returns>
    /// <response code="200">Token refresh successful</response>
    /// <response code="401">Invalid refresh token</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TokenDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 401)]
    public async Task<ActionResult<ApiResponse<TokenDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var (success, tokens, message) = await _authService.RefreshTokenAsync(request.RefreshToken);
        if (!success)
        {
            throw new UnauthorizedAccessException(message);
        }

        return Ok(ApiResponse<TokenDto>.Succeed(tokens!));
    }

    /// <summary>
    /// Revoke a refresh token
    /// </summary>
    /// <param name="request">Token to revoke</param>
    /// <returns>Success message</returns>
    /// <response code="200">Token revoked successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("revoke-token")]      
    [Authorize(Policy = "RequireUserRole")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 401)]
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