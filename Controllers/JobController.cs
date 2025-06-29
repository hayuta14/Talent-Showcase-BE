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
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;
    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpPost]
    public async Task<ActionResult<ApiResponse<JobResponseDTO>>> Create([FromBody] JobRequestDTO dto)
    {
        try
        {
            var created = await _jobService.CreateJobAsync(dto, GetUserId());
            return Ok(ApiResponse<JobResponseDTO>.Succeed(created));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<JobResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ApiResponse<JobResponseDTO>>> Update(int id, [FromBody] JobRequestDTO dto)
    {
        try
        {
            var updated = await _jobService.UpdateJobAsync(id, dto, GetUserId());
            return Ok(ApiResponse<JobResponseDTO>.Succeed(updated));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<JobResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _jobService.DeleteJobAsync(id, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }

    [HttpGet("by-user-talent")]
    public async Task<ActionResult<ApiResponse<List<JobResponseDTO>>>> GetByUserTalent()
    {
        try
        {
            var jobs = await _jobService.GetJobsByUserTalentAsync(GetUserId());
            return Ok(ApiResponse<List<JobResponseDTO>>.Succeed(jobs));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<JobResponseDTO>>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<JobResponseDTO>>> GetJobDetail(int id)
    {
        var job = await _jobService.GetJobDetailAsync(id, GetUserId());
        if (job == null) return NotFound(ApiResponse<JobResponseDTO>.Fail("Job not found"));
        return Ok(ApiResponse<JobResponseDTO>.Succeed(job));
    }
} 