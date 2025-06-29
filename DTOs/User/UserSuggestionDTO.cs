namespace TalentShowCase.API.DTOs.User;

using System.Collections.Generic;

public class UserSuggestionDTO
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public List<TalentCategoryProfileDTO> TalentCategories { get; set; } = new();
    public int FollowerCount { get; set; }
} 