using TalentShowCase.API.DTOs.Search;

namespace TalentShowCase.API.Repositories;

public interface ISearchRepository
{
    Task<(List<SearchResultDTO> Results, int TotalCount, Dictionary<string, int> TypeCounts)> SearchAsync(
        string? query, 
        string? type, 
        string? level, 
        int? talentId,
        int page, 
        int pageSize, 
        string? sortBy, 
        string? sortOrder,
        int? currentUserId = null);
    
    Task<SearchFilterDTO> GetAvailableFiltersAsync();
} 