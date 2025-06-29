using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentShowCase.API.DTOs.Common;
using TalentShowCase.API.DTOs.User;
using TalentShowCase.API.Services;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Controllers;

/// <summary>
/// User profile management controller
/// </summary>
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

    /// <summary>
    /// Get current user's profile information
    /// </summary>
    /// <returns>User profile data</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Profile not found</response>
    [HttpGet("get-profile")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 401)]
    [ProducesResponseType(typeof(ApiResponse<string>), 404)]
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

    /// <summary>
    /// Get user profile information by user ID
    /// </summary>
    /// <param name="userId">The ID of the user whose profile to retrieve</param>
    /// <returns>User profile data</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="404">Profile not found</response>
    [HttpGet("get-profile/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 404)]
    public async Task<ActionResult<ApiResponse<UserResponseDTO>>> GetProfileById(int userId)
    {
        var (success, error, user) = await _userProfileService.GetProfileByIdAsync(userId);
        if (!success)
        {
            return NotFound(ApiResponse<UserResponseDTO>.Fail(error!));
        }
        return Ok(ApiResponse<UserResponseDTO>.Succeed(user!));
    }

    /// <summary>
    /// Create or update user profile
    /// </summary>
    /// <param name="dto">Profile information to update</param>
    /// <returns>Updated profile data</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="400">Invalid profile data</response>
    /// <response code="401">Unauthorized</response>
    [HttpPatch("create-profile")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDTO>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 400)]
    [ProducesResponseType(typeof(ApiResponse<string>), 401)]
    public async Task<ActionResult<ApiResponse<UserProfileDTO>>> UpdateProfile([FromBody] CreateUserProfileDTO dto)
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