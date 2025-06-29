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
        var (items, metadata) = await _service.GetAllAsync(page, pageSize, GetUserId());
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
    public async Task<ActionResult<ApiResponse<CommentResponseDTO>>> GetComments(int postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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
    public async Task<ActionResult<ApiResponse<object>>> GetCommentCount(int postId)
    {
        var (commentCount, subCommentCount, totalCount) = await _service.GetCommentAndSubCommentCountAsync(postId);
        var data = new {
            commentCount,
            subCommentCount,
            totalCount
        };
        return Ok(ApiResponse<object>.Succeed(data));
    }

    [HttpPost("comment/{commentId}/sub-comment")]
    public async Task<ActionResult<ApiResponse<CommentResponseDTO>>> AddSubComment(int commentId, [FromBody] string content)
    {
        var (success, error, comment) = await _service.AddSubCommentAsync(commentId, GetUserId(), content);
        if (!success)
            return BadRequest(ApiResponse<CommentResponseDTO>.Fail(error!));
        return Ok(ApiResponse<CommentResponseDTO>.Succeed(comment!));
    }

    [HttpPatch("comment/{commentId}/sub-comment/{subCommentIndex}")]
    public async Task<ActionResult<ApiResponse<CommentResponseDTO>>> UpdateSubComment(int commentId, int subCommentIndex, [FromBody] string content)
    {
        var (success, error, comment) = await _service.UpdateSubCommentAsync(commentId, subCommentIndex, GetUserId(), content);
        if (!success)
            return BadRequest(ApiResponse<CommentResponseDTO>.Fail(error!));
        return Ok(ApiResponse<CommentResponseDTO>.Succeed(comment!));
    }

    [HttpDelete("comment/{commentId}/sub-comment/{subCommentIndex}")]
    public async Task<ActionResult<ApiResponse<CommentResponseDTO>>> DeleteSubComment(int commentId, int subCommentIndex)
    {
        var (success, error, comment) = await _service.DeleteSubCommentAsync(commentId, subCommentIndex, GetUserId());
        if (!success)
            return BadRequest(ApiResponse<CommentResponseDTO>.Fail(error!));
        return Ok(ApiResponse<CommentResponseDTO>.Succeed(comment!));
    }

    // Community post endpoints
    [HttpPost("community/{communityId}")]
    public async Task<ActionResult<ApiResponse<PostResponseDTO>>> CreateCommunityPost(int communityId, [FromBody] PostDTO dto)
    {
        try
        {
            var created = await _service.CreateCommunityPostAsync(dto, communityId, GetUserId());
            return Ok(ApiResponse<PostResponseDTO>.Succeed(created));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PostResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpGet("community/{communityId}")]
    public async Task<ActionResult<ApiResponse<List<PostResponseDTO>>>> GetCommunityPosts(
        int communityId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] string sortBy = "newest")
    {
        try
        {
            var posts = await _service.GetCommunityPostsAsync(communityId, page, pageSize, GetUserId(), searchTerm, categoryId, sortBy);
            return Ok(ApiResponse<List<PostResponseDTO>>.Succeed(posts));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<PostResponseDTO>>.Fail(ex.Message));
        }
    }

    [HttpGet("community/{communityId}/{postId}")]
    public async Task<ActionResult<ApiResponse<PostResponseDTO>>> GetCommunityPostById(int communityId, int postId)
    {
        try
        {
            var post = await _service.GetCommunityPostByIdAsync(postId, communityId, GetUserId());
            if (post == null)
                return NotFound(ApiResponse<PostResponseDTO>.Fail("Bài viết không tồn tại"));

            return Ok(ApiResponse<PostResponseDTO>.Succeed(post));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PostResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpGet("community/{communityId}/user/{targetUserId}")]
    public async Task<ActionResult<ApiResponse<List<PostResponseDTO>>>> GetCommunityPostsByUser(int communityId, int targetUserId)
    {
        try
        {
            var posts = await _service.GetCommunityPostsByUserAsync(communityId, targetUserId, GetUserId());
            return Ok(ApiResponse<List<PostResponseDTO>>.Succeed(posts));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<PostResponseDTO>>.Fail(ex.Message));
        }
    }

    [HttpPatch("community/{communityId}/{postId}")]
    public async Task<ActionResult<ApiResponse<PostResponseDTO>>> UpdateCommunityPost(int communityId, int postId, [FromBody] PostDTO dto)
    {
        try
        {
            var updated = await _service.UpdateCommunityPostAsync(postId, dto, communityId, GetUserId());
            return Ok(ApiResponse<PostResponseDTO>.Succeed(updated));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PostResponseDTO>.Fail(ex.Message));
        }
    }

    [HttpDelete("community/{communityId}/{postId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCommunityPost(int communityId, int postId)
    {
        try
        {
            var result = await _service.DeleteCommunityPostAsync(postId, communityId, GetUserId());
            return Ok(ApiResponse<bool>.Succeed(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
    }
} 