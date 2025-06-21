using TalentShowCase.API.DTOs.Post;
using TalentShowCase.API.Models;
using TalentShowCase.API.Repositories;
using System.Security.Claims;

namespace TalentShowCase.API.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repo;
    public PostService(IPostRepository repo)
    {
        _repo = repo;
    }

    private async Task<PostResponseDTO> ToResponseDTOAsync(Post p, int userId) => new()
    {
        Id = p.PostId,
        UserId = p.UserId,
        CategoryId = p.CategoryId,
        Description = p.Description,
        VideoUrl = p.VideoUrl ?? string.Empty,
        UserImageUrl = p.User?.ProfilePictureUrl ?? string.Empty,
        Username = p.User?.Username ?? string.Empty,
        IsPublic = p.IsPublic,
        UploadedAt = p.UploadedAt,
        LikeCount = await _repo.GetLikeCountAsync(p.PostId),
        CommentCount = await _repo.GetCommentCountAsync(p.PostId),
        LikedByCurrentUser = await _repo.ExistsLikeAsync(p.PostId, userId)
    };

    private PostResponseDTO ToResponseDTO(Post p) => new()
    {
        Id = p.PostId,
        UserId = p.UserId,
        CategoryId = p.CategoryId,
        Description = p.Description,
        VideoUrl = p.VideoUrl,
        IsPublic = p.IsPublic,
        UploadedAt = p.UploadedAt,
        LikeCount = p.Likes?.Count ?? 0,
        CommentCount = p.Comments?.Count ?? 0
    };

    public async Task<PostResponseDTO> CreateAsync(PostDTO dto, int userId)
    {
        var post = new Post
        {
            UserId = userId,
            CategoryId = dto.CategoryId,
            Description = dto.Description,
            VideoUrl = dto.VideoUrl ?? string.Empty,
            IsPublic = dto.IsPublic,
            UploadedAt = DateTime.UtcNow
        };
        var created = await _repo.CreateAsync(post);
        return await ToResponseDTOAsync(created, userId);
    }

    public async Task<PostResponseDTO?> GetByIdAsync(int id)
    {
        var p = await _repo.GetByIdAsync(id);
        return p == null ? null : await ToResponseDTOAsync(p, 0);
    }

    public async Task<PostResponseDTO> UpdateAsync(int id, PostDTO dto, int userId)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null) throw new Exception("Post not found");
        if (p.UserId != userId) throw new Exception("Unauthorized");
        p.CategoryId = dto.CategoryId;
        p.Description = dto.Description;
        p.VideoUrl = dto.VideoUrl;
        p.IsPublic = dto.IsPublic;
        var updated = await _repo.UpdateAsync(p);
        return await ToResponseDTOAsync(updated, userId);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var p = await _repo.GetByIdAsync(id);
        if (p == null || p.UserId != userId) return false;
        return await _repo.DeleteAsync(id);
    }

    public async Task<(List<PostResponseDTO> Items, object Metadata)> GetAllAsync(int page, int pageSize, int userId)
    {
        var (items, totalItems) = await _repo.GetAllAsync(page, pageSize);
        var responseItems = new List<PostResponseDTO>();
        foreach (var item in items)
        {
            responseItems.Add(await ToResponseDTOAsync(item, userId));
        }
        var metadata = new {
            page,
            pageSize,
            totalItems,
            totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
        };
        return (responseItems, metadata);
    }

    public async Task<int> GetLikeCountAsync(int postId)
    {
        return await _repo.GetLikeCountAsync(postId);
    }

    public async Task<int> GetCommentCountAsync(int postId)
    {
        return await _repo.GetCommentCountAsync(postId);
    }

    public async Task<(bool Success, string? Error)> LikePostAsync(PostLikeDTO dto, int userId)
    {
        try
        {
            if (await _repo.ExistsLikeAsync(dto.PostId, userId))
            {
                var result = await _repo.DeleteLikeAsync(dto.PostId, userId);
                if (!result)
                    return (false, "Failed to unlike post");
                return (true, null);
            }
            
            var like = new PostLike
            {
                PostId = dto.PostId,
                UserId = userId,
                LikedAt = DateTime.UtcNow
            };
            await _repo.CreateLikeAsync(like);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? Error, CommentResponseDTO? Comment)> CreateCommentAsync(CommentDTO dto, int userId)
    {
        try
        {
            var comment = new Comment
            {
                PostId = dto.PostId,
                UserId = userId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _repo.CreateCommentAsync(comment);
            var response = new CommentResponseDTO
            {
                Id = created.CommentId,
                PostId = created.PostId,
                Username = created.User?.Username ?? string.Empty,
                UserImageUrl = created.User?.ProfilePictureUrl ?? string.Empty,
                Content = created.Content,
                CreatedAt = created.CreatedAt
            };
            return (true, null, response);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<(bool Success, string? Error, object? Data, object? Metadata)> GetCommentsByPostIdAsync(int postId, int page, int pageSize)
    {
        try
        {
            var (comments, totalItems) = await _repo.GetCommentsByPostIdAsync(postId, page, pageSize);
            var response = comments.Select(c => new CommentResponseDTO
            {
                Id = c.CommentId,
                PostId = c.PostId,
                Username = c.User?.Username ?? string.Empty,
                UserImageUrl = c.User?.ProfilePictureUrl ?? string.Empty,
                Content = c.Content,
                CreatedAt = c.CreatedAt
            }).ToList();
            
            var data = new { comments = response };
            var metadata = new {
                page,
                pageSize,
                totalItems,
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize)
            };
            return (true, null, data, metadata);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null, null);
        }
    }

    public async Task<(bool Success, string? Error, CommentResponseDTO? Comment)> UpdateCommentAsync(int commentId, CommentUpdateDTO dto, int userId)
    {
        try
        {
            var comment = await _repo.GetCommentByIdAsync(commentId);
            if (comment == null)
                return (false, "Comment not found", null);
            if (comment.UserId != userId)
                return (false, "Unauthorized", null);
            
            comment.Content = dto.Content;
            var updated = await _repo.UpdateCommentAsync(comment);
            var response = new CommentResponseDTO
            {
                Id = updated.CommentId,
                PostId = updated.PostId,
                Username = updated.User?.Username ?? string.Empty,
                UserImageUrl = updated.User?.ProfilePictureUrl ?? string.Empty,
                Content = updated.Content,
                CreatedAt = updated.CreatedAt
            };
            return (true, null, response);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId, int userId)
    {
        try
        {
            var comment = await _repo.GetCommentByIdAsync(commentId);
            if (comment == null)
                return (false, "Comment not found");
            if (comment.UserId != userId)
                return (false, "Unauthorized");
            
            var result = await _repo.DeleteCommentAsync(commentId);
            if (!result)
                return (false, "Failed to delete comment");
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
} 