using System.Security.Claims;
using TalentShowCase.API.DTOs.User;
using TalentShowCase.API.Models;
using TalentShowCase.API.Repositories;
using TalentShowCase.API.Services;
using Microsoft.EntityFrameworkCore;
using TalentShowCase.API.Data;

namespace TalentShowCase.API.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IPostService _postService;
    private readonly ApplicationDbContext _context;

    public UserProfileService(IUserRepository userRepository, IPostService postService, ApplicationDbContext context)
    {
        _userRepository = userRepository;
        _postService = postService;
        _context = context;
    }

    public async Task<User?> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        return user;
    }

    public async Task<UserProfileDTO> UpdateProfileAsync(int userId, CreateUserProfileDTO dto)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) throw new Exception("User not found");
        user.Username = dto.Username;
        user.Bio = dto.Bio;
        user.ProfilePictureUrl = dto.ImageUrl;
        user.ContactInfo = dto.ContactInfo;
        // Lấy danh sách talent category hợp lệ
        var validTalentCategoryIds = await _context.TalentCategories
            .Select(tc => tc.CategoryId)
            .ToListAsync();
        var invalidIds = dto.TalentCategories
            .Select(tc => tc.Id)
            .Where(id => !validTalentCategoryIds.Contains(id))
            .ToList();
        if (invalidIds.Any())
        {
            throw new Exception($"TalentCategoryId(s) not found: {string.Join(", ", invalidIds)}");
        }
        // Lấy các talent user đã có
        var currentUserTalentCategories = await _userRepository.GetUserTalentCategoriesAsync(userId);
        var currentTalentDict = currentUserTalentCategories.ToDictionary(utc => utc.TalentCategoryId);
        var requestTalentIds = dto.TalentCategories.Select(tc => tc.Id).ToHashSet();

        // Xóa những talent không còn trong request
        var toRemove = currentUserTalentCategories.Where(utc => !requestTalentIds.Contains(utc.TalentCategoryId)).ToList();
        if (toRemove.Any())
        {
            _context.UserTalentCategories.RemoveRange(toRemove);
            await _context.SaveChangesAsync();
        }

        // Thêm mới hoặc update level
        foreach (var tc in dto.TalentCategories)
        {
            if (currentTalentDict.TryGetValue(tc.Id, out var existing))
            {
                // Update level nếu khác
                if (existing.Level != tc.Level)
                {
                    existing.Level = tc.Level;
                    _context.UserTalentCategories.Update(existing);
                }
            }
            else
            {
                // Thêm mới
                var newUtc = new UserTalentCategory
                {
                    UserId = userId,
                    TalentCategoryId = tc.Id,
                    Level = tc.Level
                };
                await _context.UserTalentCategories.AddAsync(newUtc);
            }
        }
        await _context.SaveChangesAsync();
        await _userRepository.UpdateUserAsync(user);
        // Lấy lại danh sách talent category mới nhất
        var userTalentCategories = await _userRepository.GetUserTalentCategoriesAsync(userId);
        return new UserProfileDTO
        {
            Username = user.Username,
            Bio = user.Bio ?? string.Empty,
            TalentCategories = userTalentCategories.Select(utc => new TalentCategoryProfileDTO
            {
                Id = utc.TalentCategoryId,
                Name = utc.TalentCategory?.Name ?? string.Empty,
                Level = utc.Level
            }).ToList(),
            ImageUrl = user.ProfilePictureUrl ?? string.Empty,
            ContactInfo = user.ContactInfo ?? string.Empty
        };
    }

    private UserResponseDTO ToUserResponseDTO(User user, List<TalentShowCase.API.DTOs.Post.PostResponseDTO> posts)
    {
        return new UserResponseDTO
        {
            Username = user.Username,
            Email = user.Email,
            Bio = user.Bio,
            ContactInfo = user.ContactInfo,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Posts = posts,
            TalentCategories = user.UserTalentCategories?.Select(utc => new TalentCategoryProfileDTO
            {
                Id = utc.TalentCategoryId,
                Name = utc.TalentCategory?.Name ?? string.Empty,
                Level = utc.Level
            }).ToList() ?? new List<TalentCategoryProfileDTO>(),
            PostCount = posts.Count,
            FollowerCount = user.Followers?.Count ?? 0,
            FollowingCount = user.Following?.Count ?? 0
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
        var posts = await _postService.GetPostsByUserIdAsync(userId);
        return (true, null, ToUserResponseDTO(user, posts));
    }

    public async Task<(bool Success, string? Error, UserResponseDTO? Profile)> GetProfileByIdAsync(int userId)
    {
        var user = await GetProfileAsync(userId);
        if (user == null)
            return (false, "Profile not found", null);
        var posts = await _postService.GetPostsByUserIdAsync(userId);
        return (true, null, ToUserResponseDTO(user, posts));
    }

    public async Task<(bool Success, string? Error, UserProfileDTO? Profile)> UpdateProfileAsync(ClaimsPrincipal userClaims, CreateUserProfileDTO dto)
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