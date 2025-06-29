using TalentShowCase.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TalentShowCase.API.Repositories;

public interface IFollowerRepository
{
    Task<List<User>> GetSuggestedFollowsAsync(int userId, List<int> talentCategoryIds, int topN = 10);
    Task<(bool Success, string Message)> ToggleFollowAsync(int followerId, int followedId);
    Task<bool> IsFollowingAsync(int followerId, int followedId);
    Task<int> GetFollowerCountAsync(int userId);
} 