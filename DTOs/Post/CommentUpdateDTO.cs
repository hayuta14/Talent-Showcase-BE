using System.ComponentModel.DataAnnotations;

namespace TalentShowCase.API.DTOs.Post;

public class CommentUpdateDTO
{
    [Required]
    public string Content { get; set; } = string.Empty;
} 