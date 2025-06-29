using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentShowCase.API.DTOs.Common;
using TalentShowCase.API.DTOs.Search;
using TalentShowCase.API.Services;

namespace TalentShowCase.API.Controllers;

/// <summary>
/// Search API endpoints for full-text search across users, communities, jobs, talents, and posts
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "RequireUserRole")]
[Produces("application/json")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    /// <summary>
    /// Search across all content types with filtering and pagination
    /// </summary>
    /// <param name="request">Search parameters including query, type, level, talentId, pagination, and sorting</param>
    /// <returns>Search results with metadata</returns>
    /// <response code="200">Search results returned successfully</response>
    /// <response code="400">Invalid search parameters</response>
    /// <response code="401">Unauthorized - JWT token required</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<SearchResponseDTO>), 200)]
    [ProducesResponseType(typeof(ApiResponse<SearchResponseDTO>), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<SearchResponseDTO>>> Search([FromQuery] SearchRequestDTO request)
    {
        try
        {
            // Extract current user ID from claims
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? currentUserId = null;
            if (int.TryParse(userIdStr, out var userId))
            {
                currentUserId = userId;
            }

            var result = await _searchService.SearchAsync(request, currentUserId);
            return Ok(ApiResponse<SearchResponseDTO>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<SearchResponseDTO>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Get available filters for search functionality
    /// </summary>
    /// <returns>Available content types and levels for filtering</returns>
    /// <response code="200">Filters returned successfully</response>
    /// <response code="401">Unauthorized - JWT token required</response>
    [HttpGet("filters")]
    [ProducesResponseType(typeof(ApiResponse<SearchFilterDTO>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<SearchFilterDTO>>> GetAvailableFilters()
    {
        try
        {
            var filters = await _searchService.GetAvailableFiltersAsync();
            return Ok(ApiResponse<SearchFilterDTO>.Succeed(filters));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<SearchFilterDTO>.Fail(ex.Message));
        }
    }
} 