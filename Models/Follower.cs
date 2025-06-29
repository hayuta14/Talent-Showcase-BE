using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentShowCase.API.Models;

public class Follower
{
    [Required]
    public int FollowerId { get; set; }

    [Required]
    public int FollowedId { get; set; }

    public DateTime FollowedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("FollowerId")]
    public User FollowerUser { get; set; } = null!;

    [ForeignKey("FollowedId")]
    public User FollowedUser { get; set; } = null!;
} 