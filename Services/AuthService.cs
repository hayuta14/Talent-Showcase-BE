using Microsoft.EntityFrameworkCore;
using TalentShowCase.API.Data;
using TalentShowCase.API.Models;
using TalentShowCase.API.Repositories;
using TalentShowCase.API.DTOs.Auth;

namespace TalentShowCase.API.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthRepository authRepository,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<(bool success, string message)> RegisterAsync(RegisterRequestDto request)
    {
        if (await _authRepository.GetUserByEmailAsync(request.Email) != null)
        {
            return (false, "Email already exists");
        }

        if (await _authRepository.GetUserByUsernameAsync(request.Username) != null)
        {
            return (false, "Username already exists");
        }

        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _authRepository.CreateUserAsync(user);
        _logger.LogInformation("New user registered: {Username}", user.Username);
        return (true, "Registration successful");
    }

    public async Task<(bool success, TokenDto? tokens, string message)> LoginAsync(LoginRequestDto request)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return (false, null, "Invalid email or password");
        }

        var tokens = _jwtService.GenerateTokens(user);
        
        var refreshToken = new RefreshToken
        {
            Token = tokens.RefreshToken,
            UserId = user.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = "Unknown" // Will be updated in controller
        };

        await _authRepository.CreateRefreshTokenAsync(refreshToken);
        user.LastLoginAt = DateTime.UtcNow;
        await _authRepository.UpdateUserAsync(user);

        _logger.LogInformation("User logged in: {Username}", user.Username);
        return (true, new TokenDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresAt = tokens.ExpiresAt
        }, "Login successful");
    }

    public async Task<(bool success, TokenDto? tokens, string message)> RefreshTokenAsync(string refreshToken)
    {
        var token = await _authRepository.GetRefreshTokenAsync(refreshToken);

        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
        {
            return (false, null, "Invalid refresh token");
        }

        var tokens = _jwtService.GenerateTokens(token.User!);

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.ReplacedByToken = tokens.RefreshToken;

        var newRefreshToken = new RefreshToken
        {
            Token = tokens.RefreshToken,
            UserId = token.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = "Unknown" // Will be updated in controller
        };

        await _authRepository.CreateRefreshTokenAsync(newRefreshToken);
        await _authRepository.UpdateRefreshTokenAsync(token);

        return (true, new TokenDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresAt = tokens.ExpiresAt
        }, "Token refreshed successfully");
    }

    public async Task<(bool success, string message)> RevokeTokenAsync(string refreshToken, string? reason)
    {
        var token = await _authRepository.GetRefreshTokenAsync(refreshToken);

        if (token == null)
        {
            return (false, "Token not found");
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.ReasonRevoked = reason;

        await _authRepository.UpdateRefreshTokenAsync(token);
        return (true, "Token revoked successfully");
    }
} 