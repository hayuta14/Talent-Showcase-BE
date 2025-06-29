using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentShowCase.API.Models;

public class Comment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CommentId { get; set; }

    [Required]
    public int PostId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Thêm property cho sub comment dạng JSON
    public List<SubComment> SubComments { get; set; } = new List<SubComment>();

    // Navigation properties
    [ForeignKey("PostId")]
    public Post Post { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}

// Thêm class SubComment
public class SubComment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 