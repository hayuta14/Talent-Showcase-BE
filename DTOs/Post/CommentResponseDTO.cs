namespace TalentShowCase.API.DTOs.Post;

public class CommentResponseDTO
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string UserImageUrl { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<SubCommentResponseDTO> SubComments { get; set; } = new List<SubCommentResponseDTO>();
}

public class SubCommentResponseDTO
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string UserImageUrl { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 