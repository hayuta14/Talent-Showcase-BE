using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentShowCase.API.DTOs.Common;
using TalentShowCase.API.DTOs.User;
using TalentShowCase.API.Services;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "RequireUserRole")]
public class UserController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UserController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    [HttpGet("get-profile")]
    public async Task<ActionResult<ApiResponse<UserResponseDTO>>> GetProfile()
    {
        var (success, error, user) = await _userProfileService.GetProfileAsync(User);
        if (!success)
        {
            if (error == "Invalid user id")
                return Unauthorized(ApiResponse<UserResponseDTO>.Fail(error!));
            return NotFound(ApiResponse<UserResponseDTO>.Fail(error!));
        }
        return Ok(ApiResponse<UserResponseDTO>.Succeed(user!));
    }

    [HttpPatch("create-profile")]
    public async Task<ActionResult<ApiResponse<UserProfileDTO>>> UpdateProfile([FromBody] UserProfileDTO dto)
    {
        var (success, error, updated) = await _userProfileService.UpdateProfileAsync(User, dto);
        if (!success)
        {
            if (error == "Invalid user id")
                return Unauthorized(ApiResponse<UserProfileDTO>.Fail(error!));
            return BadRequest(ApiResponse<UserProfileDTO>.Fail(error!));
        }
        return Ok(ApiResponse<UserProfileDTO>.Succeed(updated!));
    }
} 