using System.ComponentModel.DataAnnotations.Schema;

namespace TalentShowCase.API.Models;

public class UserTalentCategory
{
    public int UserId { get; set; }
    public int TalentCategoryId { get; set; }
    public string Level { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    [ForeignKey("TalentCategoryId")]
    public TalentCategory TalentCategory { get; set; } = null!;
} 