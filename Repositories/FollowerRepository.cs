using Microsoft.EntityFrameworkCore;
using TalentShowCase.API.Data;
using TalentShowCase.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace TalentShowCase.API.Repositories;

public class FollowerRepository : IFollowerRepository
{
    private readonly ApplicationDbContext _context;
    public FollowerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetSuggestedFollowsAsync(int userId, List<int> talentCategoryIds, int topN = 10)
    {
        // Lấy talentCategoryIds của userId nếu không truyền vào
        if (talentCategoryIds == null || talentCategoryIds.Count == 0)
        {
            talentCategoryIds = await _context.UserTalentCategories
                .Where(utc => utc.UserId == userId)
                .Select(utc => utc.TalentCategoryId)
                .ToListAsync();
        }
        var followingIds = await _context.Followers.Where(f => f.FollowerId == userId).Select(f => f.FollowedId).ToListAsync();
        var suggested = await _context.Users
            .Where(u => u.UserId != userId && !followingIds.Contains(u.UserId))
            .Include(u => u.UserTalentCategories)
                .ThenInclude(utc => utc.TalentCategory)
            .Include(u => u.Followers)
            .Select(u => new {
                User = u,
                MatchCount = u.UserTalentCategories.Count(utc => talentCategoryIds.Contains(utc.TalentCategoryId)),
                FollowerCount = u.Followers.Count
            })
            .Where(x => x.MatchCount > 0)
            .OrderByDescending(x => x.MatchCount)
            .ThenByDescending(x => x.FollowerCount)
            .Take(topN)
            .Select(x => x.User)
            .ToListAsync();
        return suggested;
    }

    public async Task<(bool Success, string Message)> ToggleFollowAsync(int followerId, int followedId)
    {
        try
        {
            if (followerId == followedId) return (false, "Cannot follow yourself");
            var existing = await _context.Followers.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
            if (existing != null)
            {
                _context.Followers.Remove(existing);
                await _context.SaveChangesAsync();
                return (true, "Unfollow success");
            }
            else
            {
                var newFollow = new Follower { FollowerId = followerId, FollowedId = followedId };
                await _context.Followers.AddAsync(newFollow);
                await _context.SaveChangesAsync();
                return (true, "Follow success");
            }
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }

    public async Task<bool> IsFollowingAsync(int followerId, int followedId)
    {
        return await _context.Followers.AnyAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
    }

    public async Task<int> GetFollowerCountAsync(int userId)
    {
        return await _context.Followers.CountAsync(f => f.FollowedId == userId);
    }
} 