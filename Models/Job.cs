using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentShowCase.API.Models;

public class Job
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int JobId { get; set; }

    [Required]
    [StringLength(100)]
    public string JobTitle { get; set; } = string.Empty;

    [StringLength(100)]
    public string? CompanyName { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    [StringLength(50)]
    public string? Salary { get; set; }

    public string? JobDescription { get; set; }
    public string? Requirements { get; set; }
    public DateTime? ExpireAt { get; set; }

    // Quan hệ many-to-many với TalentCategory
    public ICollection<JobTalentCategory> JobTalentCategories { get; set; } = new List<JobTalentCategory>();

    // Quan hệ one-to-many với JobApplication
    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
} 