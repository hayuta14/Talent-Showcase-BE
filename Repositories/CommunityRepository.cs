using Microsoft.EntityFrameworkCore;
using TalentShowCase.API.Data;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public class CommunityRepository : ICommunityRepository
{
    private readonly ApplicationDbContext _context;

    public CommunityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Community> CreateCommunityAsync(Community community)
    {
        _context.Communities.Add(community);
        await _context.SaveChangesAsync();
        return community;
    }

    public async Task<Community?> GetCommunityByIdAsync(int communityId)
    {
        return await _context.Communities
            .Include(c => c.Creator)
            .FirstOrDefaultAsync(c => c.CommunityId == communityId);
    }

    public async Task<Community?> GetCommunityWithMembersAsync(int communityId)
    {
        return await _context.Communities
            .Include(c => c.Creator)
            .Include(c => c.Members)
            .ThenInclude(cm => cm.User)
            .FirstOrDefaultAsync(c => c.CommunityId == communityId);
    }

    public async Task<List<Community>> GetCommunitiesAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Communities
            .Include(c => c.Creator)
            .Include(c => c.Members)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Name.Contains(searchTerm) || c.Description!.Contains(searchTerm));
        }

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Community>> GetCommunitiesByUserAsync(int userId)
    {
        return await _context.Communities
            .Include(c => c.Creator)
            .Include(c => c.Members)
            .Where(c => c.CreatorId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Community>> GetCommunitiesUserIsMemberOfAsync(int userId)
    {
        return await _context.Communities
            .Include(c => c.Creator)
            .Include(c => c.Members)
            .Where(c => c.Members.Any(cm => cm.UserId == userId))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Community> UpdateCommunityAsync(Community community)
    {
        _context.Communities.Update(community);
        await _context.SaveChangesAsync();
        return community;
    }

    public async Task<bool> DeleteCommunityAsync(int communityId)
    {
        var community = await _context.Communities.FindAsync(communityId);
        if (community == null) return false;

        _context.Communities.Remove(community);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CommunityMember> AddMemberAsync(CommunityMember member)
    {
        _context.CommunityMembers.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<CommunityMember?> GetMemberAsync(int communityId, int userId)
    {
        return await _context.CommunityMembers
            .Include(cm => cm.User)
            .FirstOrDefaultAsync(cm => cm.CommunityId == communityId && cm.UserId == userId);
    }

    public async Task<List<CommunityMember>> GetCommunityMembersAsync(int communityId)
    {
        return await _context.CommunityMembers
            .Include(cm => cm.User)
            .Where(cm => cm.CommunityId == communityId)
            .OrderBy(cm => cm.JoinedAt)
            .ToListAsync();
    }

    public async Task<CommunityMember> UpdateMemberRoleAsync(CommunityMember member)
    {
        _context.CommunityMembers.Update(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<bool> RemoveMemberAsync(int communityId, int userId)
    {
        var member = await _context.CommunityMembers
            .FirstOrDefaultAsync(cm => cm.CommunityId == communityId && cm.UserId == userId);
        
        if (member == null) return false;

        _context.CommunityMembers.Remove(member);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsUserMemberAsync(int communityId, int userId)
    {
        return await _context.CommunityMembers
            .AnyAsync(cm => cm.CommunityId == communityId && cm.UserId == userId);
    }

    public async Task<bool> IsUserCreatorAsync(int communityId, int userId)
    {
        return await _context.Communities
            .AnyAsync(c => c.CommunityId == communityId && c.CreatorId == userId);
    }

    public async Task<bool> IsUserAdminAsync(int communityId, int userId)
    {
        return await _context.CommunityMembers
            .AnyAsync(cm => cm.CommunityId == communityId && cm.UserId == userId && 
                           (cm.Role == "admin" || cm.Role == "creator"));
    }

    public async Task<int> GetCommunityMemberCountAsync(int communityId)
    {
        return await _context.CommunityMembers
            .CountAsync(cm => cm.CommunityId == communityId);
    }
} 