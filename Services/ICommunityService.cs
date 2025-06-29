using TalentShowCase.API.DTOs.Community;

namespace TalentShowCase.API.Services;

public interface ICommunityService
{
    Task<CommunityResponseDTO> CreateCommunityAsync(CreateCommunityRequestDTO dto, int userId);
    Task<CommunityResponseDTO?> GetCommunityByIdAsync(int communityId, int userId);
    Task<CommunityDetailResponseDTO?> GetCommunityDetailAsync(int communityId, int userId);
    Task<List<CommunityResponseDTO>> GetCommunitiesAsync(CommunitySearchRequestDTO dto, int userId = 0);
    Task<List<CommunityResponseDTO>> GetMyCommunitiesAsync(int userId);
    Task<List<CommunityResponseDTO>> GetCommunitiesIJoinedAsync(int userId);
    Task<CommunityResponseDTO> UpdateCommunityAsync(int communityId, UpdateCommunityRequestDTO dto, int userId);
    Task<bool> DeleteCommunityAsync(int communityId, int userId);
    Task<bool> JoinCommunityAsync(JoinCommunityRequestDTO dto, int userId);
    Task<bool> LeaveCommunityAsync(int communityId, int userId);
    Task<List<CommunityMemberDTO>> GetCommunityMembersAsync(int communityId, int userId);
    Task<CommunityMemberDTO> UpdateMemberRoleAsync(int communityId, UpdateMemberRoleRequestDTO dto, int adminUserId);
    Task<bool> RemoveMemberAsync(int communityId, int memberUserId, int adminUserId);
    Task<bool> IsUserMemberAsync(int communityId, int userId);
    Task<bool> IsUserCreatorAsync(int communityId, int userId);
    Task<bool> IsUserAdminAsync(int communityId, int userId);
} 