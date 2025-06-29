namespace TalentShowCase.API.DTOs.Job;

public class JobResponseDTO
{
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Location { get; set; }
    public string? Salary { get; set; }
    public string? JobDescription { get; set; }
    public string? Requirements { get; set; }
    public DateTime? ExpireAt { get; set; }
    public List<TalentShowCase.API.DTOs.Common.TalentCategoryLevelDTO> TalentCategories { get; set; } = new();
    public int UserId { get; set; }
    public string? UserName { get; set; }
} 