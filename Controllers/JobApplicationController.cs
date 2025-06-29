using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentShowCase.API.DTOs.Common;
using TalentShowCase.API.DTOs.Job;
using TalentShowCase.API.Services;

namespace TalentShowCase.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "RequireUserRole")]
public class JobApplicationController : ControllerBase
{
    private readonly IJobApplicationService _applicationService;
    
    public JobApplicationController(IJobApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpPost("apply")]
    public async Task<ActionResult<ApiResponse<JobApplicationResponseDTO>>> ApplyToJob([FromBody] JobApplicationRequestDTO dto)
    {
        try
        {
            var application = await _applicationService.ApplyToJobAsync(dto, GetUserId());
            return Ok(ApiResponse<JobApplicationResponseDTO>.Succeed(application));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<JobApplicationResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpGet("my-applications")]
    public async Task<ActionResult<ApiResponse<List<JobApplicationResponseDTO>>>> GetMyApplications()
    {
        try
        {
            var applications = await _applicationService.GetMyApplicationsAsync(GetUserId());
            return Ok(ApiResponse<List<JobApplicationResponseDTO>>.Succeed(applications));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<JobApplicationResponseDTO>>.Fail(ex.Message));
        }
    }

    [HttpGet("job/{jobId}/applications")]
    public async Task<ActionResult<ApiResponse<List<JobApplicationResponseDTO>>>> GetApplicationsForJob(int jobId)
    {
        try
        {
            var applications = await _applicationService.GetApplicationsForMyJobAsync(jobId, GetUserId());
            return Ok(ApiResponse<List<JobApplicationResponseDTO>>.Succeed(applications));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<JobApplicationResponseDTO>>.Fail(ex.Message));
        }
    }

    [HttpGet("{applicationId}")]
    public async Task<ActionResult<ApiResponse<JobApplicationResponseDTO>>> GetApplicationById(int applicationId)
    {
        try
        {
            var application = await _applicationService.GetApplicationByIdAsync(applicationId, GetUserId());
            return Ok(ApiResponse<JobApplicationResponseDTO>.Succeed(application));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<JobApplicationResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpPatch("{applicationId}/status")]
    public async Task<ActionResult<ApiResponse<JobApplicationResponseDTO>>> UpdateApplicationStatus(
        int applicationId, 
        [FromBody] UpdateApplicationStatusDTO dto)
    {
        try
        {
            var application = await _applicationService.UpdateApplicationStatusAsync(applicationId, dto, GetUserId());
            return Ok(ApiResponse<JobApplicationResponseDTO>.Succeed(application));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<JobApplicationResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpDelete("{applicationId}/withdraw")]
    public async Task<ActionResult<ApiResponse<bool>>> WithdrawApplication(int applicationId)
    {
        try
        {
            var result = await _applicationService.WithdrawApplicationAsync(applicationId, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }

    [HttpGet("job/{jobId}/has-applied")]
    public async Task<ActionResult<ApiResponse<bool>>> HasAppliedToJob(int jobId)
    {
        try
        {
            var hasApplied = await _applicationService.HasAppliedToJobAsync(jobId, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(hasApplied));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }
} 