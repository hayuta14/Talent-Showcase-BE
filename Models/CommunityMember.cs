using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentShowCase.API.Models;

public class CommunityMember
{
    [Required]
    public int CommunityId { get; set; }

    [Required]
    public int UserId { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [StringLength(20)]
    public string Role { get; set; } = "member";

    // Navigation properties
    [ForeignKey("CommunityId")]
    public Community Community { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
} 