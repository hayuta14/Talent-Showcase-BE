using System.ComponentModel.DataAnnotations;
using TalentShowCase.API.DTOs.Common;
namespace TalentShowCase.API.DTOs.User;

public class UserProfileDTO
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Bio { get; set; } = string.Empty;

    [Required]
    public List<TalentCategoryProfileDTO> TalentCategories { get; set; } = new();
    
    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public string ContactInfo { get; set; } = string.Empty;
}

public class TalentCategoryProfileDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}

public class CreateUserProfileDTO
{
    public string Username { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public List<TalentCategoryLevelDTO> TalentCategories { get; set; } = new();
    public string ImageUrl { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
}