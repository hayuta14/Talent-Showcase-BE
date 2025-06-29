using Microsoft.EntityFrameworkCore;
using TalentShowCase.API.Data;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.UserTalentCategories)
                .ThenInclude(utc => utc.TalentCategory)
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<List<UserTalentCategory>> GetUserTalentCategoriesAsync(int userId)
    {
        return await _context.UserTalentCategories
            .Where(utc => utc.UserId == userId)
            .Include(utc => utc.TalentCategory)
            .ToListAsync();
    }

    public async Task UpdateUserTalentCategoriesAsync(int userId, List<UserTalentCategory> userTalentCategories)
    {
        var existingList = _context.UserTalentCategories.Where(utc => utc.UserId == userId).ToList();
        var existingDict = existingList.ToDictionary(utc => utc.TalentCategoryId);

        foreach (var utc in userTalentCategories)
        {
            utc.User = null!;
            utc.TalentCategory = null!;
            if (existingDict.TryGetValue(utc.TalentCategoryId, out var existing))
            {
                // Update level nếu khác
                if (existing.Level != utc.Level)
                {
                    existing.Level = utc.Level;
                    _context.UserTalentCategories.Update(existing);
                }
            }
            else
            {
                // Thêm mới
                await _context.UserTalentCategories.AddAsync(utc);
            }
        }
        await _context.SaveChangesAsync();
    }
} 