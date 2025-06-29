using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentShowCase.API.Models;

public class JobApplication
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ApplicationId { get; set; }

    [Required]
    public int JobId { get; set; }

    [Required]
    public int ApplicantId { get; set; }

    [Required]
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    public string? CoverLetter { get; set; }

    public string? ResumeUrl { get; set; }

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string? Notes { get; set; } // Notes from employer

    // Navigation properties
    [ForeignKey("JobId")]
    public Job Job { get; set; } = null!;

    [ForeignKey("ApplicantId")]
    public User Applicant { get; set; } = null!;
}

public enum ApplicationStatus
{
    Pending = 0,        // Đã apply, chờ review
    UnderReview = 1,    // Đang review
    Shortlisted = 2,    // Đã shortlist
    Interview = 3,      // Mời phỏng vấn
    Rejected = 4,       // Từ chối
    Accepted = 5,       // Chấp nhận
    Withdrawn = 6       // Rút đơn
} 