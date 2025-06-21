using System.ComponentModel.DataAnnotations;

namespace TalentShowCase.API.DTOs.Post;

public class PostLikeDTO
{
    [Required]
    public int PostId { get; set; }
} 