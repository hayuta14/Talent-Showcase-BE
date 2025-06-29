using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentShowCase.API.DTOs.Common;
using TalentShowCase.API.DTOs.Community;
using TalentShowCase.API.Services;

namespace TalentShowCase.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "RequireUserRole")]
public class CommunityController : ControllerBase
{
    private readonly ICommunityService _communityService;
    
    public CommunityController(ICommunityService communityService)
    {
        _communityService = communityService;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CommunityResponseDTO>>> CreateCommunity([FromBody] CreateCommunityRequestDTO dto)
    {
        try
        {
            var community = await _communityService.CreateCommunityAsync(dto, GetUserId());
            return Ok(ApiResponse<CommunityResponseDTO>.Succeed(community));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommunityResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CommunityResponseDTO>>>> GetCommunities([FromQuery] CommunitySearchRequestDTO dto)
    {
        try
        {
            var communities = await _communityService.GetCommunitiesAsync(dto, GetUserId());
            return Ok(ApiResponse<List<CommunityResponseDTO>>.Succeed(communities));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<CommunityResponseDTO>>.Fail(ex.Message));
        }
    }

    [HttpGet("my-communities")]
    public async Task<ActionResult<ApiResponse<List<CommunityResponseDTO>>>> GetMyCommunities()
    {
        try
        {
            var communities = await _communityService.GetMyCommunitiesAsync(GetUserId());
            return Ok(ApiResponse<List<CommunityResponseDTO>>.Succeed(communities));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<CommunityResponseDTO>>.Fail(ex.Message));
        }
    }

    [HttpGet("joined-communities")]
    public async Task<ActionResult<ApiResponse<List<CommunityResponseDTO>>>> GetJoinedCommunities()
    {
        try
        {
            var communities = await _communityService.GetCommunitiesIJoinedAsync(GetUserId());
            return Ok(ApiResponse<List<CommunityResponseDTO>>.Succeed(communities));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<CommunityResponseDTO>>.Fail(ex.Message));
        }
    }

    [HttpGet("{communityId}")]
    public async Task<ActionResult<ApiResponse<CommunityResponseDTO>>> GetCommunityById(int communityId)
    {
        try
        {
            var community = await _communityService.GetCommunityByIdAsync(communityId, GetUserId());
            if (community == null)
                return NotFound(ApiResponse<CommunityResponseDTO>.Fail("Community không tồn tại"));

            return Ok(ApiResponse<CommunityResponseDTO>.Succeed(community));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommunityResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpGet("{communityId}/detail")]
    public async Task<ActionResult<ApiResponse<CommunityDetailResponseDTO>>> GetCommunityDetail(int communityId)
    {
        try
        {
            var community = await _communityService.GetCommunityDetailAsync(communityId, GetUserId());
            if (community == null)
                return NotFound(ApiResponse<CommunityDetailResponseDTO>.Fail("Community không tồn tại"));

            return Ok(ApiResponse<CommunityDetailResponseDTO>.Succeed(community));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommunityDetailResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpPut("{communityId}")]
    public async Task<ActionResult<ApiResponse<CommunityResponseDTO>>> UpdateCommunity(int communityId, [FromBody] UpdateCommunityRequestDTO dto)
    {
        try
        {
            var community = await _communityService.UpdateCommunityAsync(communityId, dto, GetUserId());
            return Ok(ApiResponse<CommunityResponseDTO>.Succeed(community));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommunityResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpDelete("{communityId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCommunity(int communityId)
    {
        try
        {
            var result = await _communityService.DeleteCommunityAsync(communityId, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }

    [HttpPost("join")]
    public async Task<ActionResult<ApiResponse<bool>>> JoinCommunity([FromBody] JoinCommunityRequestDTO dto)
    {
        try
        {
            var result = await _communityService.JoinCommunityAsync(dto, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }

    [HttpDelete("{communityId}/leave")]
    public async Task<ActionResult<ApiResponse<bool>>> LeaveCommunity(int communityId)
    {
        try
        {
            var result = await _communityService.LeaveCommunityAsync(communityId, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }

    [HttpGet("{communityId}/members")]
    public async Task<ActionResult<ApiResponse<List<CommunityMemberDTO>>>> GetCommunityMembers(int communityId)
    {
        try
        {
            var members = await _communityService.GetCommunityMembersAsync(communityId, GetUserId());
            return Ok(ApiResponse<List<CommunityMemberDTO>>.Succeed(members));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<CommunityMemberDTO>>.Fail(ex.Message));
        }
    }

    [HttpPatch("{communityId}/members/role")]
    public async Task<ActionResult<ApiResponse<CommunityMemberDTO>>> UpdateMemberRole(int communityId, [FromBody] UpdateMemberRoleRequestDTO dto)
    {
        try
        {
            var member = await _communityService.UpdateMemberRoleAsync(communityId, dto, GetUserId());
            return Ok(ApiResponse<CommunityMemberDTO>.Succeed(member));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommunityMemberDTO>.Fail(ex.Message));
        }
    }

    [HttpDelete("{communityId}/members/{memberUserId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveMember(int communityId, int memberUserId)
    {
        try
        {
            var result = await _communityService.RemoveMemberAsync(communityId, memberUserId, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }

    [HttpGet("{communityId}/is-member")]
    public async Task<ActionResult<ApiResponse<bool>>> IsUserMember(int communityId)
    {
        try
        {
            var result = await _communityService.IsUserMemberAsync(communityId, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }

    [HttpGet("{communityId}/is-creator")]
    public async Task<ActionResult<ApiResponse<bool>>> IsUserCreator(int communityId)
    {
        try
        {
            var result = await _communityService.IsUserCreatorAsync(communityId, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }

    [HttpGet("{communityId}/is-admin")]
    public async Task<ActionResult<ApiResponse<bool>>> IsUserAdmin(int communityId)
    {
        try
        {
            var result = await _communityService.IsUserAdminAsync(communityId, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }
} 