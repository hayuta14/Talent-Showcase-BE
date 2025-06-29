namespace TalentShowCase.API.DTOs.User;

public class UserResponseDTO
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Skill { get; set; }
    public string? ContactInfo { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public List<TalentShowCase.API.DTOs.Post.PostResponseDTO> Posts { get; set; } = new();
    public List<TalentCategoryProfileDTO> TalentCategories { get; set; } = new();
    public int PostCount { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
} 