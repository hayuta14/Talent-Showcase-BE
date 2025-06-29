using System.ComponentModel.DataAnnotations.Schema;

namespace TalentShowCase.API.Models;

public class JobTalentCategory
{
    public int JobId { get; set; }
    public int TalentCategoryId { get; set; }
    public string Level { get; set; } = string.Empty;

    [ForeignKey("JobId")]
    public Job Job { get; set; } = null!;
    [ForeignKey("TalentCategoryId")]
    public TalentCategory TalentCategory { get; set; } = null!;
} 