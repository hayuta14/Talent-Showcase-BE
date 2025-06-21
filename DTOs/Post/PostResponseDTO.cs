namespace TalentShowCase.API.DTOs.Post;

public class PostResponseDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public DateTime UploadedAt { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
} 