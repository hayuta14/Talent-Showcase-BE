namespace TalentShowCase.API.DTOs.Post;

public class CommentResponseDTO
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 