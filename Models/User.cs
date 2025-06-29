using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentShowCase.API.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    public string? Bio { get; set; }

    [StringLength(255)]
    public string? ContactInfo { get; set; }

    [StringLength(255)]
    public string? ProfilePictureUrl { get; set; }

    [Required]
    public int RoleId { get; set; } = 2; // Default role ID (User)

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey("RoleId")]
    public Role Role { get; set; } = null!;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Community> CreatedCommunities { get; set; } = new List<Community>();
    public ICollection<CommunityMember> CommunityMemberships { get; set; } = new List<CommunityMember>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public ICollection<Follower> Followers { get; set; } = new List<Follower>();
    public ICollection<Follower> Following { get; set; } = new List<Follower>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<TalentCategory> TalentCategories { get; set; } = new List<TalentCategory>();
    public ICollection<UserTalentCategory> UserTalentCategories { get; set; } = new List<UserTalentCategory>();
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
} 