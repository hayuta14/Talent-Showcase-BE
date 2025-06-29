using Microsoft.AspNetCore.Mvc;
using TalentShowCase.API.DTOs.Category;
using TalentShowCase.API.DTOs.Common;
using TalentShowCase.API.Services;

namespace TalentShowCase.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;
    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryResponseDTO>>> Create([FromBody] CategoryDTO dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return Ok(ApiResponse<CategoryResponseDTO>.Succeed(created));
        }
        catch (Exception ex)
        {
            if (ex.Message == "Category name already exists")
                return Conflict(ApiResponse<CategoryResponseDTO>.Fail(ex.Message, 409));
            return BadRequest(ApiResponse<CategoryResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryResponseDTO>>> GetById(int id)
    {
        var cat = await _service.GetByIdAsync(id);
        if (cat == null) return NotFound(ApiResponse<CategoryResponseDTO>.Fail("Not found"));
        return Ok(ApiResponse<CategoryResponseDTO>.Succeed(cat));
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryResponseDTO>>> Update(int id, [FromBody] CategoryDTO dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<CategoryResponseDTO>.Succeed(updated));
        }
        catch (Exception ex)
        {
            if (ex.Message == "Category name already exists")
                return Conflict(ApiResponse<CategoryResponseDTO>.Fail(ex.Message,409));
            return NotFound(ApiResponse<CategoryResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound(ApiResponse<bool>.Fail("Not found"));
        return Ok(ApiResponse<bool>.Succeed(true));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (items, metadata) = await _service.GetAllAsync(page, pageSize);
        var data = new { items };
        return Ok(ApiResponse<object>.Succeed(data, 200, metadata));
    }
} 