using Microsoft.EntityFrameworkCore;
using TalentShowCase.API.Data;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TalentCategory> CreateAsync(TalentCategory category)
    {
        _context.TalentCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<TalentCategory?> GetByIdAsync(int id)
    {
        return await _context.TalentCategories.FindAsync(id);
    }

    public async Task<TalentCategory> UpdateAsync(TalentCategory category)
    {
        _context.TalentCategories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.TalentCategories.FindAsync(id);
        if (category == null) return false;
        _context.TalentCategories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(List<TalentCategory> Items, int TotalItems)> GetAllAsync(int page, int pageSize)
    {
        var query = _context.TalentCategories.AsQueryable();
        var totalItems = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalItems);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.TalentCategories.AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }
} 