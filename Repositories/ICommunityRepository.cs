using TalentShowCase.API.DTOs.Community;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public interface ICommunityRepository
{
    Task<Community> CreateCommunityAsync(Community community);
    Task<Community?> GetCommunityByIdAsync(int communityId);
    Task<Community?> GetCommunityWithMembersAsync(int communityId);
    Task<List<Community>> GetCommunitiesAsync(int page, int pageSize, string? searchTerm = null);
    Task<List<Community>> GetCommunitiesByUserAsync(int userId);
    Task<List<Community>> GetCommunitiesUserIsMemberOfAsync(int userId);
    Task<Community> UpdateCommunityAsync(Community community);
    Task<bool> DeleteCommunityAsync(int communityId);
    Task<CommunityMember> AddMemberAsync(CommunityMember member);
    Task<CommunityMember?> GetMemberAsync(int communityId, int userId);
    Task<List<CommunityMember>> GetCommunityMembersAsync(int communityId);
    Task<CommunityMember> UpdateMemberRoleAsync(CommunityMember member);
    Task<bool> RemoveMemberAsync(int communityId, int userId);
    Task<bool> IsUserMemberAsync(int communityId, int userId);
    Task<bool> IsUserCreatorAsync(int communityId, int userId);
    Task<bool> IsUserAdminAsync(int communityId, int userId);
    Task<int> GetCommunityMemberCountAsync(int communityId);
} 