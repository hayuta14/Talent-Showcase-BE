using TalentShowCase.API.DTOs.Category;
using TalentShowCase.API.Models;
using TalentShowCase.API.Repositories;

namespace TalentShowCase.API.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    private CategoryResponseDTO ToResponseDTO(TalentCategory c) => new()
    {
        Id = c.CategoryId,
        Name = c.Name,
        Description = c.Description
    };

    public async Task<CategoryResponseDTO> CreateAsync(CategoryDTO dto)
    {
        if (await _repo.ExistsByNameAsync(dto.Name))
            throw new Exception("Category name already exists");
        var category = new TalentCategory
        {
            Name = dto.Name,
            Description = dto.Description
        };
        var created = await _repo.CreateAsync(category);
        return ToResponseDTO(created);
    }

    public async Task<CategoryResponseDTO?> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        return c == null ? null : ToResponseDTO(c);
    }

    public async Task<CategoryResponseDTO> UpdateAsync(int id, CategoryDTO dto)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) throw new Exception("Category not found");
        if (!string.Equals(c.Name, dto.Name, StringComparison.OrdinalIgnoreCase) && await _repo.ExistsByNameAsync(dto.Name))
            throw new Exception("Category name already exists");
        c.Name = dto.Name;
        c.Description = dto.Description;
        var updated = await _repo.UpdateAsync(c);
        return ToResponseDTO(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repo.DeleteAsync(id);
    }

    public async Task<(List<CategoryResponseDTO> Items, object Metadata)> GetAllAsync(int page, int pageSize)
    {
        var (items, totalItems) = await _repo.GetAllAsync(page, pageSize);
        var metadata = new {
            page,
            pageSize,
            totalItems,
            totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
        };
        return (items.Select(ToResponseDTO).ToList(), metadata);
    }
} 