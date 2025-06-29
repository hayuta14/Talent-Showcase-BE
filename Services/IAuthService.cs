using TalentShowCase.API.Models;
using TalentShowCase.API.DTOs.Auth;

namespace TalentShowCase.API.Services;

public interface IAuthService
{
    Task<(bool success, string message)> RegisterAsync(RegisterRequestDto request);
    Task<(bool success, TokenDto? tokens, string message)> LoginAsync(LoginRequestDto request);
    Task<(bool success, TokenDto? tokens, string message)> RefreshTokenAsync(string refreshToken);
    Task<(bool success, string message)> RevokeTokenAsync(string refreshToken, string? reason);
} 