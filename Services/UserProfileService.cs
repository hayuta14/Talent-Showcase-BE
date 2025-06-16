using System.Security.Claims;
using TalentShowCase.API.DTOs.User;
using TalentShowCase.API.Models;
using TalentShowCase.API.Repositories;

namespace TalentShowCase.API.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;

    public UserProfileService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) return null;
        return user;
    }

    public async Task<UserProfileDTO> UpdateProfileAsync(int userId, UserProfileDTO dto)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) throw new Exception("User not found");
        user.Bio = dto.Bio;
        user.Skill = dto.Skill;
        user.ProfilePictureUrl = dto.ImageUrl;
        await _userRepository.UpdateUserAsync(user);
        return new UserProfileDTO
        {
            Bio = user.Bio ?? string.Empty,
            Skill = user.Skill ?? string.Empty,
            ImageUrl = user.ProfilePictureUrl ?? string.Empty
        };
    }

    private UserResponseDTO ToUserResponseDTO(User user)
    {
        return new UserResponseDTO
        {
            Username = user.Username,
            Email = user.Email,
            Bio = user.Bio,
            Skill = user.Skill,
            ContactInfo = user.ContactInfo,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    public async Task<(bool Success, string? Error, UserResponseDTO? Profile)> GetProfileAsync(ClaimsPrincipal userClaims)
    {
        var userIdStr = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId))
            return (false, "Invalid user id", null);
        var user = await GetProfileAsync(userId);
        if (user == null)
            return (false, "Profile not found", null);
        return (true, null, ToUserResponseDTO(user));
    }

    public async Task<(bool Success, string? Error, UserProfileDTO? Profile)> UpdateProfileAsync(ClaimsPrincipal userClaims, UserProfileDTO dto)
    {
        var userIdStr = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId))
            return (false, "Invalid user id", null);
        try
        {
            var updated = await UpdateProfileAsync(userId, dto);
            return (true, null, updated);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }
} 