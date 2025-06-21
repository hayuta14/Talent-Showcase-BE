using System.ComponentModel.DataAnnotations;
namespace TalentShowCase.API.DTOs.User;

public class UserProfileDTO
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Bio { get; set; } = string.Empty;

    [Required]
    public string Skill { get; set; } = string.Empty;
    
    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public string ContactInfo { get; set; } = string.Empty;
}