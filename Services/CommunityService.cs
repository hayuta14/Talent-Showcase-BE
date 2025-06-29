using TalentShowCase.API.DTOs.Community;
using TalentShowCase.API.Models;
using TalentShowCase.API.Repositories;

namespace TalentShowCase.API.Services;

public class CommunityService : ICommunityService
{
    private readonly ICommunityRepository _communityRepository;

    public CommunityService(ICommunityRepository communityRepository)
    {
        _communityRepository = communityRepository;
    }

    public async Task<CommunityResponseDTO> CreateCommunityAsync(CreateCommunityRequestDTO dto, int userId)
    {
        var community = new Community
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatorId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var createdCommunity = await _communityRepository.CreateCommunityAsync(community);

        // Add creator as first member with "creator" role
        var creatorMember = new CommunityMember
        {
            CommunityId = createdCommunity.CommunityId,
            UserId = userId,
            Role = "creator",
            JoinedAt = DateTime.UtcNow
        };

        await _communityRepository.AddMemberAsync(creatorMember);

        return await MapToResponseDTO(createdCommunity, userId);
    }

    public async Task<CommunityResponseDTO?> GetCommunityByIdAsync(int communityId, int userId)
    {
        var community = await _communityRepository.GetCommunityByIdAsync(communityId);
        if (community == null) return null;

        return await MapToResponseDTO(community, userId);
    }

    public async Task<CommunityDetailResponseDTO?> GetCommunityDetailAsync(int communityId, int userId)
    {
        var community = await _communityRepository.GetCommunityWithMembersAsync(communityId);
        if (community == null) return null;

        var isMember = await _communityRepository.IsUserMemberAsync(communityId, userId);
        var userRole = isMember ? (await _communityRepository.GetMemberAsync(communityId, userId))?.Role : null;

        return new CommunityDetailResponseDTO
        {
            CommunityId = community.CommunityId,
            Name = community.Name,
            Description = community.Description,
            CreatorId = community.CreatorId,
            CreatorName = community.Creator.Username,
            CreatedAt = community.CreatedAt,
            MemberCount = community.Members.Count,
            IsMember = isMember,
            UserRole = userRole,
            Members = community.Members.Select(MapToMemberDTO).ToList()
        };
    }

    public async Task<List<CommunityResponseDTO>> GetCommunitiesAsync(CommunitySearchRequestDTO dto, int userId = 0)
    {
        var communities = await _communityRepository.GetCommunitiesAsync(dto.Page, dto.PageSize, dto.SearchTerm);
        var result = new List<CommunityResponseDTO>();

        foreach (var community in communities)
        {
            result.Add(await MapToResponseDTO(community, userId));
        }

        return result;
    }

    public async Task<List<CommunityResponseDTO>> GetMyCommunitiesAsync(int userId)
    {
        var communities = await _communityRepository.GetCommunitiesByUserAsync(userId);
        var result = new List<CommunityResponseDTO>();

        foreach (var community in communities)
        {
            result.Add(await MapToResponseDTO(community, userId));
        }

        return result;
    }

    public async Task<List<CommunityResponseDTO>> GetCommunitiesIJoinedAsync(int userId)
    {
        var communities = await _communityRepository.GetCommunitiesUserIsMemberOfAsync(userId);
        var result = new List<CommunityResponseDTO>();

        foreach (var community in communities)
        {
            result.Add(await MapToResponseDTO(community, userId));
        }

        return result;
    }

    public async Task<CommunityResponseDTO> UpdateCommunityAsync(int communityId, UpdateCommunityRequestDTO dto, int userId)
    {
        var community = await _communityRepository.GetCommunityByIdAsync(communityId);
        if (community == null)
            throw new Exception("Community không tồn tại");

        if (!await _communityRepository.IsUserCreatorAsync(communityId, userId))
            throw new Exception("Chỉ creator mới có thể cập nhật community");

        community.Name = dto.Name;
        community.Description = dto.Description;

        var updatedCommunity = await _communityRepository.UpdateCommunityAsync(community);
        return await MapToResponseDTO(updatedCommunity, userId);
    }

    public async Task<bool> DeleteCommunityAsync(int communityId, int userId)
    {
        if (!await _communityRepository.IsUserCreatorAsync(communityId, userId))
            throw new Exception("Chỉ creator mới có thể xóa community");

        return await _communityRepository.DeleteCommunityAsync(communityId);
    }

    public async Task<bool> JoinCommunityAsync(JoinCommunityRequestDTO dto, int userId)
    {
        var community = await _communityRepository.GetCommunityByIdAsync(dto.CommunityId);
        if (community == null)
            throw new Exception("Community không tồn tại");

        var isAlreadyMember = await _communityRepository.IsUserMemberAsync(dto.CommunityId, userId);
        if (isAlreadyMember)
            throw new Exception("Bạn đã là thành viên của community này");

        var member = new CommunityMember
        {
            CommunityId = dto.CommunityId,
            UserId = userId,
            Role = "member",
            JoinedAt = DateTime.UtcNow
        };

        await _communityRepository.AddMemberAsync(member);
        return true;
    }

    public async Task<bool> LeaveCommunityAsync(int communityId, int userId)
    {
        var isCreator = await _communityRepository.IsUserCreatorAsync(communityId, userId);
        if (isCreator)
            throw new Exception("Creator không thể rời community. Hãy xóa community hoặc chuyển quyền creator");

        return await _communityRepository.RemoveMemberAsync(communityId, userId);
    }

    public async Task<List<CommunityMemberDTO>> GetCommunityMembersAsync(int communityId, int userId)
    {
        var isMember = await _communityRepository.IsUserMemberAsync(communityId, userId);
        if (!isMember)
            throw new Exception("Bạn phải là thành viên để xem danh sách thành viên");

        var members = await _communityRepository.GetCommunityMembersAsync(communityId);
        return members.Select(MapToMemberDTO).ToList();
    }

    public async Task<CommunityMemberDTO> UpdateMemberRoleAsync(int communityId, UpdateMemberRoleRequestDTO dto, int adminUserId)
    {
        var isAdmin = await _communityRepository.IsUserAdminAsync(communityId, adminUserId);
        if (!isAdmin)
            throw new Exception("Chỉ admin mới có thể cập nhật role thành viên");

        var member = await _communityRepository.GetMemberAsync(communityId, dto.UserId);
        if (member == null)
            throw new Exception("Thành viên không tồn tại");

        member.Role = dto.Role;
        var updatedMember = await _communityRepository.UpdateMemberRoleAsync(member);
        return MapToMemberDTO(updatedMember);
    }

    public async Task<bool> RemoveMemberAsync(int communityId, int memberUserId, int adminUserId)
    {
        var isAdmin = await _communityRepository.IsUserAdminAsync(communityId, adminUserId);
        if (!isAdmin)
            throw new Exception("Chỉ admin mới có thể xóa thành viên");

        var isCreator = await _communityRepository.IsUserCreatorAsync(communityId, memberUserId);
        if (isCreator)
            throw new Exception("Không thể xóa creator khỏi community");

        return await _communityRepository.RemoveMemberAsync(communityId, memberUserId);
    }

    public async Task<bool> IsUserMemberAsync(int communityId, int userId)
    {
        return await _communityRepository.IsUserMemberAsync(communityId, userId);
    }

    public async Task<bool> IsUserCreatorAsync(int communityId, int userId)
    {
        return await _communityRepository.IsUserCreatorAsync(communityId, userId);
    }

    public async Task<bool> IsUserAdminAsync(int communityId, int userId)
    {
        return await _communityRepository.IsUserAdminAsync(communityId, userId);
    }

    private async Task<CommunityResponseDTO> MapToResponseDTO(Community community, int userId)
    {
        var isMember = userId > 0 ? await _communityRepository.IsUserMemberAsync(community.CommunityId, userId) : false;
        var userRole = isMember ? (await _communityRepository.GetMemberAsync(community.CommunityId, userId))?.Role : null;
        var memberCount = await _communityRepository.GetCommunityMemberCountAsync(community.CommunityId);

        return new CommunityResponseDTO
        {
            CommunityId = community.CommunityId,
            Name = community.Name,
            Description = community.Description,
            CreatorId = community.CreatorId,
            CreatorName = community.Creator.Username,
            CreatedAt = community.CreatedAt,
            MemberCount = memberCount,
            IsMember = isMember,
            UserRole = userRole
        };
    }

    private static CommunityMemberDTO MapToMemberDTO(CommunityMember member)
    {
        return new CommunityMemberDTO
        {
            UserId = member.UserId,
            Username = member.User.Username,
            Email = member.User.Email,
            ProfilePictureUrl = member.User.ProfilePictureUrl,
            Role = member.Role,
            JoinedAt = member.JoinedAt
        };
    }
} 