using System.ComponentModel.DataAnnotations;

namespace TalentShowCase.API.DTOs.Auth;

public class RevokeTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
    public string? Reason { get; set; }
} 