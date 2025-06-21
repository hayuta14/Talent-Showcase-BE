using System.ComponentModel.DataAnnotations;

namespace TalentShowCase.API.DTOs.Post;

public class CommentDTO
{
    [Required]
    public int PostId { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
} 