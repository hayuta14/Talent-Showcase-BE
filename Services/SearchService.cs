using TalentShowCase.API.DTOs.Search;
using TalentShowCase.API.Repositories;
using System.ComponentModel.DataAnnotations;

namespace TalentShowCase.API.Services;

public class SearchService : ISearchService
{
    private readonly ISearchRepository _searchRepository;

    public SearchService(ISearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<SearchResponseDTO> SearchAsync(SearchRequestDTO request, int? currentUserId = null)
    {
        // Validate request
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);
        
        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
            throw new ArgumentException($"Validation failed: {errors}");
        }

        // Normalize and validate parameters
        if (request.Page < 1) request.Page = 1;
        if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 10;

        // Normalize sort parameters
        var sortBy = NormalizeSortBy(request.SortBy);
        var sortOrder = NormalizeSortOrder(request.SortOrder);

        // Validate search query
        if (!string.IsNullOrEmpty(request.Query) && request.Query.Length > 200)
        {
            throw new ArgumentException("Search query cannot exceed 200 characters");
        }

        // Perform search
        var (results, totalCount, typeCounts) = await _searchRepository.SearchAsync(
            request.Query,
            request.Type,
            request.Level,
            request.TalentId,
            request.Page,
            request.PageSize,
            sortBy,
            sortOrder,
            currentUserId
        );

        // Calculate metadata
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var metadata = new SearchMetadataDTO
        {
            TotalResults = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = totalPages,
            TypeCounts = typeCounts
        };

        return new SearchResponseDTO
        {
            Results = results,
            Metadata = metadata
        };
    }

    public async Task<SearchFilterDTO> GetAvailableFiltersAsync()
    {
        return await _searchRepository.GetAvailableFiltersAsync();
    }

    private static string? NormalizeSortBy(string? sortBy)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "relevance" => "relevance",
            "date" => "date",
            "name" => "name",
            _ => "relevance"
        };
    }

    private static string? NormalizeSortOrder(string? sortOrder)
    {
        return sortOrder?.ToLowerInvariant() switch
        {
            "asc" => "asc",
            "desc" => "desc",
            _ => "desc"
        };
    }
} 