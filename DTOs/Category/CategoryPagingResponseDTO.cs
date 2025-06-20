using System.Collections.Generic;

namespace TalentShowCase.API.DTOs.Category;

public class CategoryPagingResponseDTO
{
    public List<CategoryResponseDTO> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
} 