using System.Collections.Generic;
using System.Threading.Tasks;
using TalentShowCase.API.Models;
using TalentShowCase.API.DTOs.User;

namespace TalentShowCase.API.Services;

public interface IFollowerService
{
    Task<List<UserSuggestionDTO>> GetSuggestedFollowsAsync(int userId, List<int> talentCategoryIds, int topN = 10);
    Task<(bool Success, string Message)> ToggleFollowAsync(int followerId, int followedId);
    Task<bool> IsFollowingAsync(int followerId, int followedId);
    Task<int> GetFollowerCountAsync(int userId);
} 