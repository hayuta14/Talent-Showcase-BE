using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentShowCase.API.Services;
using TalentShowCase.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalentShowCase.API.DTOs.User;

namespace TalentShowCase.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "RequireUserRole")]
public class FollowerController : ControllerBase
{
    private readonly IFollowerService _service;
    public FollowerController(IFollowerService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet("suggested")] // ?talentCategoryIds=1,2,3&topN=10
    public async Task<ActionResult<List<UserSuggestionDTO>>> GetSuggested([FromQuery] List<int> talentCategoryIds, [FromQuery] int topN = 10)
    {
        var users = await _service.GetSuggestedFollowsAsync(GetUserId(), talentCategoryIds, topN);
        return Ok(users);
    }

    [HttpPost("toggle/{followedId}")]
    public async Task<ActionResult<object>> ToggleFollow(int followedId)
    {
        var (success, message) = await _service.ToggleFollowAsync(GetUserId(), followedId);
        if (!success) return BadRequest(new { success, message });
        return Ok(new { success, message });
    }

    [HttpGet("is-following/{followedId}")]
    public async Task<ActionResult<bool>> IsFollowing(int followedId)
    {
        var result = await _service.IsFollowingAsync(GetUserId(), followedId);
        return Ok(result);
    }

    [HttpGet("count/{userId}")]
    public async Task<ActionResult<int>> GetFollowerCount(int userId)
    {
        var count = await _service.GetFollowerCountAsync(userId);
        return Ok(count);
    }
} 