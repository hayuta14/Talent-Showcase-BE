using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentShowCase.API.DTOs.Common;
using TalentShowCase.API.DTOs.Post;
using TalentShowCase.API.Services;

namespace TalentShowCase.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "RequireUserRole")]
public class PostController : ControllerBase
{
    private readonly IPostService _service;
    public PostController(IPostService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PostResponseDTO>>> Create([FromBody] PostDTO dto)
    {
        var created = await _service.CreateAsync(dto, GetUserId());
        return Ok(ApiResponse<PostResponseDTO>.Succeed(created));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PostResponseDTO>>> GetById(int id)
    {
        var post = await _service.GetByIdAsync(id);
        if (post == null) return NotFound(ApiResponse<PostResponseDTO>.Fail("Not found"));
        return Ok(ApiResponse<PostResponseDTO>.Succeed(post));
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ApiResponse<PostResponseDTO>>> Update(int id, [FromBody] PostDTO dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto, GetUserId());
            return Ok(ApiResponse<PostResponseDTO>.Succeed(updated));
        }
        catch (Exception ex)
        {
            if (ex.Message == "Unauthorized")
                return Forbid();
            return NotFound(ApiResponse<PostResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var result = await _service.DeleteAsync(id, GetUserId());
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

    [HttpPost("like")]
    public async Task<ActionResult<ApiResponse<string>>> LikePost([FromBody] PostLikeDTO dto)
    {
        var (success, error) = await _service.LikePostAsync(dto, GetUserId());
        if (!success)
            return BadRequest(ApiResponse<string>.Fail(error!));
        return Ok(ApiResponse<string>.Succeed("Post like toggled successfully"));
    }

    [HttpPost("comment")]
    public async Task<ActionResult<ApiResponse<CommentResponseDTO>>> CreateComment([FromBody] CommentDTO dto)
    {
        var (success, error, comment) = await _service.CreateCommentAsync(dto, GetUserId());
        if (!success)
            return BadRequest(ApiResponse<CommentResponseDTO>.Fail(error!));
        return Ok(ApiResponse<CommentResponseDTO>.Succeed(comment!));
    }

    [HttpPatch("comment/{commentId}")]
    public async Task<ActionResult<ApiResponse<CommentResponseDTO>>> UpdateComment(int commentId, [FromBody] CommentUpdateDTO dto)
    {
        var (success, error, comment) = await _service.UpdateCommentAsync(commentId, dto, GetUserId());
        if (!success)
        {
            if (error == "Unauthorized")
                return Forbid();
            if (error == "Comment not found")
                return NotFound(ApiResponse<CommentResponseDTO>.Fail(error!));
            return BadRequest(ApiResponse<CommentResponseDTO>.Fail(error!));
        }
        return Ok(ApiResponse<CommentResponseDTO>.Succeed(comment!));
    }

    [HttpDelete("comment/{commentId}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteComment(int commentId)
    {
        var (success, error) = await _service.DeleteCommentAsync(commentId, GetUserId());
        if (!success)
        {
            if (error == "Unauthorized")
                return Forbid();
            if (error == "Comment not found")
                return NotFound(ApiResponse<string>.Fail(error!));
            return BadRequest(ApiResponse<string>.Fail(error!));
        }
        return Ok(ApiResponse<string>.Succeed("Comment deleted successfully"));
    }

    [HttpGet("{postId}/comments")]
    public async Task<ActionResult<ApiResponse<object>>> GetComments(int postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (success, error, data, metadata) = await _service.GetCommentsByPostIdAsync(postId, page, pageSize);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(error!));
        return Ok(ApiResponse<object>.Succeed(data!, 200, metadata));
    }

    [HttpGet("{postId}/like-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetLikeCount(int postId)
    {
        var count = await _service.GetLikeCountAsync(postId);
        return Ok(ApiResponse<int>.Succeed(count));
    }

    [HttpGet("{postId}/comment-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetCommentCount(int postId)
    {
        var count = await _service.GetCommentCountAsync(postId);
        return Ok(ApiResponse<int>.Succeed(count));
    }
} 