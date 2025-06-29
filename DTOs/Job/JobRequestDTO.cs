using TalentShowCase.API.DTOs.Common;

namespace TalentShowCase.API.DTOs.Job;

public class JobRequestDTO
{
    public string JobTitle { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Location { get; set; }
    public string? Salary { get; set; }
    public string? JobDescription { get; set; }
    public string? Requirements { get; set; }
    public DateTime? ExpireAt { get; set; }
    public List<int> TalentCategoryIds { get; set; } = new();
    public List<string> Levels { get; set; } = new();
} 