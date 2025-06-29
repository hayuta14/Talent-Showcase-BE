using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> UpdateUserAsync(User user);
    Task<List<UserTalentCategory>> GetUserTalentCategoriesAsync(int userId);
    Task UpdateUserTalentCategoriesAsync(int userId, List<UserTalentCategory> userTalentCategories);
} 