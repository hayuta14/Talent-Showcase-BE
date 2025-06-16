using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentShowCase.API.Models;

public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PostId { get; set; }

    [Required]
    public int UserId { get; set; }

    public int? CategoryId { get; set; }

    [Required]
    [StringLength(100)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string VideoUrl { get; set; } = string.Empty;

    public bool IsPublic { get; set; } = true;

    public int? SharedFromPostId { get; set; }

    public int? CommunityId { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public TalentCategory? Category { get; set; }

    [ForeignKey("SharedFromPostId")]
    public Post? SharedFromPost { get; set; }

    [ForeignKey("CommunityId")]
    public Community? Community { get; set; }

    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Post> SharedPosts { get; set; } = new List<Post>();
} 