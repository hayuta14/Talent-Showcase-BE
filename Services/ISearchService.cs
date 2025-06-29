using TalentShowCase.API.DTOs.Search;

namespace TalentShowCase.API.Services;

public interface ISearchService
{
    Task<SearchResponseDTO> SearchAsync(SearchRequestDTO request, int? currentUserId = null);
    Task<SearchFilterDTO> GetAvailableFiltersAsync();
} 