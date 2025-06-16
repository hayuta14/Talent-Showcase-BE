using System.ComponentModel.DataAnnotations;
namespace TalentShowCase.API.DTOs.User;

public class UserProfileDTO
{
    [Required]
    public string Bio { get; set; } = string.Empty;

    [Required]
    public string Skill { get; set; }
    
    [Required]
    public string ImageUrl { get; set; }

}