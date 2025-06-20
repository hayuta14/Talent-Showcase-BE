using TalentShowCase.API.DTOs.Category;

namespace TalentShowCase.API.Services;

public interface ICategoryService
{
    Task<CategoryResponseDTO> CreateAsync(CategoryDTO dto);
    Task<CategoryResponseDTO?> GetByIdAsync(int id);
    Task<CategoryResponseDTO> UpdateAsync(int id, CategoryDTO dto);
    Task<bool> DeleteAsync(int id);
    Task<(List<CategoryResponseDTO> Items, object Metadata)> GetAllAsync(int page, int pageSize);
} 