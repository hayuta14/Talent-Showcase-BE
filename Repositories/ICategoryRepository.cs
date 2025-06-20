using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public interface ICategoryRepository
{
    Task<TalentCategory> CreateAsync(TalentCategory category);
    Task<TalentCategory?> GetByIdAsync(int id);
    Task<TalentCategory> UpdateAsync(TalentCategory category);
    Task<bool> DeleteAsync(int id);
    Task<(List<TalentCategory> Items, int TotalItems)> GetAllAsync(int page, int pageSize);
    Task<bool> ExistsByNameAsync(string name);
} 