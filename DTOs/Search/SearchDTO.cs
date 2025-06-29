using System.ComponentModel.DataAnnotations;

namespace TalentShowCase.API.DTOs.Search;

/// <summary>
/// Request DTO for search functionality
/// </summary>
public class SearchRequestDTO
{
    /// <summary>
    /// Search query string to search across all fields
    /// </summary>
    /// <example>music</example>
    public string? Query { get; set; }

    /// <summary>
    /// Type of content to search for. Options: "user", "community", "job", "talent", "post", "all"
    /// </summary>
    /// <example>all</example>
    [RegularExpression("^(user|community|job|talent|post|all)$", ErrorMessage = "Type must be one of: user, community, job, talent, post, all")]
    public string? Type { get; set; } = "user";

    /// <summary>
    /// Filter by talent level (for jobs and talents)
    /// </summary>
    /// <example>expert</example>
    public string? Level { get; set; }

    /// <summary>
    /// Filter by talent category ID (for jobs, posts, talents)
    /// </summary>
    /// <example>1</example>
    public int? TalentId { get; set; }

    /// <summary>
    /// Page number for pagination (starts from 1)
    /// </summary>
    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page (max 100)
    /// </summary>
    /// <example>10</example>
    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Sort field. Options: "relevance", "date", "name"
    /// </summary>
    /// <example>relevance</example>
    [RegularExpression("^(relevance|date|name)$", ErrorMessage = "SortBy must be one of: relevance, date, name")]
    public string? SortBy { get; set; } = "date";

    /// <summary>
    /// Sort order. Options: "asc", "desc"
    /// </summary>
    /// <example>desc</example>
    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortOrder must be one of: asc, desc")]
    public string? SortOrder { get; set; } = "desc";
}

/// <summary>
/// Response DTO for search results
/// </summary>
public class SearchResponseDTO
{
    /// <summary>
    /// List of search results
    /// </summary>
    public List<SearchResultDTO> Results { get; set; } = new();

    /// <summary>
    /// Search metadata including pagination info
    /// </summary>
    public SearchMetadataDTO Metadata { get; set; } = new();
}

/// <summary>
/// Individual search result item
/// </summary>
public class SearchResultDTO
{
    /// <summary>
    /// Type of the result: "user", "community", "job", "talent", "post"
    /// </summary>
    /// <example>user</example>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier of the item
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Title or name of the item
    /// </summary>
    /// <example>John Doe</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description or bio of the item
    /// </summary>
    /// <example>Professional musician with 10 years of experience</example>
    public string? Description { get; set; }

    /// <summary>
    /// URL to the image or video thumbnail
    /// </summary>
    /// <example>https://example.com/image.jpg</example>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Creation date of the item
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Relevance score for search ranking
    /// </summary>
    /// <example>1.0</example>
    public double RelevanceScore { get; set; }

    /// <summary>
    /// Additional data specific to the result type
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Metadata for search results including pagination
/// </summary>
public class SearchMetadataDTO
{
    /// <summary>
    /// Total number of results found
    /// </summary>
    /// <example>150</example>
    public int TotalResults { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    /// <example>1</example>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    /// <example>15</example>
    public int TotalPages { get; set; }

    /// <summary>
    /// Count of results by type
    /// </summary>
    /// <example>{"user": 50, "community": 30, "job": 40, "talent": 20, "post": 10}</example>
    public Dictionary<string, int> TypeCounts { get; set; } = new();
}

/// <summary>
/// Available filters for search
/// </summary>
public class SearchFilterDTO
{
    /// <summary>
    /// Available content types for filtering
    /// </summary>
    /// <example>["user", "community", "job", "talent", "post"]</example>
    public List<string> Types { get; set; } = new();

    /// <summary>
    /// Available talent levels for filtering
    /// </summary>
    /// <example>["beginner", "intermediate", "expert", "master"]</example>
    public List<string> Levels { get; set; } = new();

    /// <summary>
    /// Start date for date range filtering
    /// </summary>
    /// <example>2024-01-01T00:00:00Z</example>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// End date for date range filtering
    /// </summary>
    /// <example>2024-12-31T23:59:59Z</example>
    public DateTime? DateTo { get; set; }
} 