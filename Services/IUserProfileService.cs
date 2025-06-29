using System.Security.Claims;
using TalentShowCase.API.DTOs.User;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Services;

public interface IUserProfileService
{
    Task<User?> GetProfileAsync(int userId);

    // New for controller
    Task<(bool Success, string? Error, UserResponseDTO? Profile)> GetProfileAsync(ClaimsPrincipal userClaims);
    Task<(bool Success, string? Error, UserResponseDTO? Profile)> GetProfileByIdAsync(int userId);
    Task<(bool Success, string? Error, UserProfileDTO? Profile)> UpdateProfileAsync(ClaimsPrincipal userClaims, CreateUserProfileDTO dto);
} 