using TalentShowCase.API.Models;

namespace TalentShowCase.API.DTOs.Job;

public class JobApplicationRequestDTO
{
    public int JobId { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
}

public class JobApplicationResponseDTO
{
    public int ApplicationId { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int ApplicantId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Notes { get; set; }
}

public class UpdateApplicationStatusDTO
{
    public ApplicationStatus Status { get; set; }
    public string? Notes { get; set; }
} 