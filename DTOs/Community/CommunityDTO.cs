using TalentShowCase.API.Models;

namespace TalentShowCase.API.DTOs.Community;

public class CreateCommunityRequestDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCommunityRequestDTO
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CommunityResponseDTO
{
    public int CommunityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CreatorId { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int MemberCount { get; set; }
    public bool IsMember { get; set; }
    public string? UserRole { get; set; }
}

public class CommunityDetailResponseDTO
{
    public int CommunityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CreatorId { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<CommunityMemberDTO> Members { get; set; } = new();
    public int MemberCount { get; set; }
    public bool IsMember { get; set; }
    public string? UserRole { get; set; }
}

public class CommunityMemberDTO
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}

public class JoinCommunityRequestDTO
{
    public int CommunityId { get; set; }
}

public class UpdateMemberRoleRequestDTO
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class CommunitySearchRequestDTO
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 