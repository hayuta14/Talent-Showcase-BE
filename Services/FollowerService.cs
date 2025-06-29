using System.Collections.Generic;
using System.Threading.Tasks;
using TalentShowCase.API.Models;
using TalentShowCase.API.Repositories;
using TalentShowCase.API.DTOs.User;

namespace TalentShowCase.API.Services;

public class FollowerService : IFollowerService
{
    private readonly IFollowerRepository _repo;
    public FollowerService(IFollowerRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<UserSuggestionDTO>> GetSuggestedFollowsAsync(int userId, List<int> talentCategoryIds, int topN = 10)
    {
        var users = await _repo.GetSuggestedFollowsAsync(userId, talentCategoryIds, topN);
        return users.Select(u => new UserSuggestionDTO
        {
            UserId = u.UserId,
            Username = u.Username,
            ProfilePictureUrl = u.ProfilePictureUrl,
            TalentCategories = u.UserTalentCategories?.Select(utc => new TalentCategoryProfileDTO
            {
                Id = utc.TalentCategoryId,
                Name = utc.TalentCategory?.Name ?? string.Empty,
                Level = utc.Level
            }).ToList() ?? new List<TalentCategoryProfileDTO>(),
            FollowerCount = u.Followers?.Count ?? 0
        }).ToList();
    }

    public Task<(bool Success, string Message)> ToggleFollowAsync(int followerId, int followedId)
        => _repo.ToggleFollowAsync(followerId, followedId);

    public Task<bool> IsFollowingAsync(int followerId, int followedId)
        => _repo.IsFollowingAsync(followerId, followedId);

    public Task<int> GetFollowerCountAsync(int userId)
        => _repo.GetFollowerCountAsync(userId);
} 