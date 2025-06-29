using TalentShowCase.API.DTOs.Post;
using TalentShowCase.API.Models;
using TalentShowCase.API.Repositories;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TalentShowCase.API.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repo;
    public PostService(IPostRepository repo)
    {
        _repo = repo;
    }

    private async Task<PostResponseDTO> ToResponseDTOAsync(Post p, int userId)
    {
        var (commentCount, subCommentCount, totalCount) = await GetCommentAndSubCommentCountAsync(p.PostId);
        return new PostResponseDTO
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
            CommentCount = totalCount,
            LikedByCurrentUser = await _repo.ExistsLikeAsync(p.PostId, userId)
        };
    }

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
            // Lấy thông tin user cho sub comment
            var userDict = await _repo.GetUserDictionaryByIds(created.SubComments?.Select(sc => sc.UserId).Distinct().ToList() ?? new List<int>());
            var response = new CommentResponseDTO
            {
                Id = created.CommentId,
                PostId = created.PostId,
                Username = created.User?.Username ?? string.Empty,
                UserImageUrl = created.User?.ProfilePictureUrl ?? string.Empty,
                Content = created.Content,
                CreatedAt = created.CreatedAt,
                SubComments = created.SubComments?.Select(sc => new SubCommentResponseDTO
                {
                    UserId = sc.UserId,
                    Username = userDict.TryGetValue(sc.UserId, out var u) ? u.Username : string.Empty,
                    UserImageUrl = userDict.TryGetValue(sc.UserId, out var u2) ? u2.ProfilePictureUrl ?? string.Empty : string.Empty,
                    Content = sc.Content,
                    CreatedAt = sc.CreatedAt
                }).ToList() ?? new List<SubCommentResponseDTO>()
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
            // Lấy tất cả userId của sub comment
            var allSubUserIds = comments.SelectMany(c => c.SubComments ?? new List<SubComment>()).Select(sc => sc.UserId).Distinct().ToList();
            var userDict = await _repo.GetUserDictionaryByIds(allSubUserIds);
            var response = comments.Select(c => new CommentResponseDTO
            {
                Id = c.CommentId,
                PostId = c.PostId,
                Username = c.User?.Username ?? string.Empty,
                UserImageUrl = c.User?.ProfilePictureUrl ?? string.Empty,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                SubComments = c.SubComments?.Select(sc => new SubCommentResponseDTO
                {
                    UserId = sc.UserId,
                    Username = userDict.TryGetValue(sc.UserId, out var u) ? u.Username : string.Empty,
                    UserImageUrl = userDict.TryGetValue(sc.UserId, out var u2) ? u2.ProfilePictureUrl ?? string.Empty : string.Empty,
                    Content = sc.Content,
                    CreatedAt = sc.CreatedAt
                }).ToList() ?? new List<SubCommentResponseDTO>()
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
                CreatedAt = updated.CreatedAt,
                SubComments = updated.SubComments?.Select(sc => new SubCommentResponseDTO
                {
                    UserId = sc.UserId,
                    Content = sc.Content,
                    CreatedAt = sc.CreatedAt
                }).ToList() ?? new List<SubCommentResponseDTO>()
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

    public async Task<List<PostResponseDTO>> GetPostsByUserIdAsync(int userId)
    {
        var posts = await _repo.GetPostsByUserIdAsync(userId);
        var result = new List<PostResponseDTO>();
        foreach (var post in posts)
        {
            result.Add(await ToResponseDTOAsync(post, userId));
        }
        return result;
    }

    public async Task<(bool Success, string? Error, CommentResponseDTO? Comment)> AddSubCommentAsync(int commentId, int userId, string content)
    {
        var comment = await _repo.GetCommentByIdAsync(commentId);
        if (comment == null)
            return (false, "Comment not found", null);
        var subComment = new SubComment
        {
            UserId = userId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };
        var updated = await _repo.AddSubCommentAsync(commentId, subComment);
        if (updated == null)
            return (false, "Failed to add sub comment", null);
        // Lấy lại thông tin user nếu cần
        var user = updated.User;
        var response = new CommentResponseDTO
        {
            Id = updated.CommentId,
            PostId = updated.PostId,
            Username = user?.Username ?? string.Empty,
            UserImageUrl = user?.ProfilePictureUrl ?? string.Empty,
            Content = updated.Content,
            CreatedAt = updated.CreatedAt,
            SubComments = updated.SubComments?.Select(sc => new SubCommentResponseDTO
            {
                UserId = sc.UserId,
                Content = sc.Content,
                CreatedAt = sc.CreatedAt
            }).ToList() ?? new List<SubCommentResponseDTO>()
        };
        return (true, null, response);
    }

    public async Task<(bool Success, string? Error, CommentResponseDTO? Comment)> UpdateSubCommentAsync(int commentId, int subCommentIndex, int userId, string content)
    {
        var comment = await _repo.GetCommentByIdAsync(commentId);
        if (comment == null || comment.SubComments == null || subCommentIndex < 0 || subCommentIndex >= comment.SubComments.Count)
            return (false, "Sub comment not found", null);
        if (comment.SubComments[subCommentIndex].UserId != userId)
            return (false, "Unauthorized", null);
        var updated = await _repo.UpdateSubCommentAsync(commentId, subCommentIndex, content);
        if (updated == null)
            return (false, "Failed to update sub comment", null);
        var user = updated.User;
        var response = new CommentResponseDTO
        {
            Id = updated.CommentId,
            PostId = updated.PostId,
            Username = user?.Username ?? string.Empty,
            UserImageUrl = user?.ProfilePictureUrl ?? string.Empty,
            Content = updated.Content,
            CreatedAt = updated.CreatedAt,
            SubComments = updated.SubComments?.Select(sc => new SubCommentResponseDTO
            {
                UserId = sc.UserId,
                Content = sc.Content,
                CreatedAt = sc.CreatedAt
            }).ToList() ?? new List<SubCommentResponseDTO>()
        };
        return (true, null, response);
    }

    public async Task<(bool Success, string? Error, CommentResponseDTO? Comment)> DeleteSubCommentAsync(int commentId, int subCommentIndex, int userId)
    {
        var comment = await _repo.GetCommentByIdAsync(commentId);
        if (comment == null || comment.SubComments == null || subCommentIndex < 0 || subCommentIndex >= comment.SubComments.Count)
            return (false, "Sub comment not found", null);
        if (comment.SubComments[subCommentIndex].UserId != userId)
            return (false, "Unauthorized", null);
        var updated = await _repo.DeleteSubCommentAsync(commentId, subCommentIndex);
        if (updated == null)
            return (false, "Failed to delete sub comment", null);
        var user = updated.User;
        var response = new CommentResponseDTO
        {
            Id = updated.CommentId,
            PostId = updated.PostId,
            Username = user?.Username ?? string.Empty,
            UserImageUrl = user?.ProfilePictureUrl ?? string.Empty,
            Content = updated.Content,
            CreatedAt = updated.CreatedAt,
            SubComments = updated.SubComments?.Select(sc => new SubCommentResponseDTO
            {
                UserId = sc.UserId,
                Content = sc.Content,
                CreatedAt = sc.CreatedAt
            }).ToList() ?? new List<SubCommentResponseDTO>()
        };
        return (true, null, response);
    }

    public async Task<(int CommentCount, int SubCommentCount, int TotalCount)> GetCommentAndSubCommentCountAsync(int postId)
    {
        var comments = await _repo.GetCommentsByPostIdAsync(postId, 1, int.MaxValue);
        var commentCount = comments.Item1.Count;
        var subCommentCount = comments.Item1.Sum(c => c.SubComments?.Count ?? 0);
        var totalCount = commentCount + subCommentCount;
        return (commentCount, subCommentCount, totalCount);
    }

    // Community post methods
    public async Task<PostResponseDTO> CreateCommunityPostAsync(PostDTO dto, int communityId, int userId)
    {
        var post = new Post
        {
            UserId = userId,
            CategoryId = dto.CategoryId,
            Description = dto.Description,
            VideoUrl = dto.VideoUrl ?? string.Empty,
            IsPublic = false, // Community posts are always private to members
            CommunityId = communityId,
            UploadedAt = DateTime.UtcNow
        };
        var created = await _repo.CreateAsync(post);
        return await ToResponseDTOAsync(created, userId);
    }

    public async Task<PostResponseDTO?> GetCommunityPostByIdAsync(int postId, int communityId, int userId)
    {
        var post = await _repo.GetByIdAsync(postId);
        if (post == null || post.CommunityId != communityId) return null;
        
        return await ToResponseDTOAsync(post, userId);
    }

    public async Task<List<PostResponseDTO>> GetCommunityPostsAsync(int communityId, int page, int pageSize, int userId, string? searchTerm = null, int? categoryId = null, string sortBy = "newest")
    {
        var posts = await _repo.GetCommunityPostsAsync(communityId, page, pageSize, searchTerm, categoryId, sortBy);
        var result = new List<PostResponseDTO>();
        
        foreach (var post in posts)
        {
            result.Add(await ToResponseDTOAsync(post, userId));
        }
        
        return result;
    }

    public async Task<List<PostResponseDTO>> GetCommunityPostsByUserAsync(int communityId, int targetUserId, int currentUserId)
    {
        var posts = await _repo.GetCommunityPostsByUserAsync(communityId, targetUserId);
        var result = new List<PostResponseDTO>();
        
        foreach (var post in posts)
        {
            result.Add(await ToResponseDTOAsync(post, currentUserId));
        }
        
        return result;
    }

    public async Task<PostResponseDTO> UpdateCommunityPostAsync(int postId, PostDTO dto, int communityId, int userId)
    {
        var post = await _repo.GetByIdAsync(postId);
        if (post == null || post.CommunityId != communityId)
            throw new Exception("Post not found in community");
        
        if (post.UserId != userId)
            throw new Exception("Unauthorized to update this post");
        
        post.CategoryId = dto.CategoryId;
        post.Description = dto.Description;
        post.VideoUrl = dto.VideoUrl;
        
        var updated = await _repo.UpdateAsync(post);
        return await ToResponseDTOAsync(updated, userId);
    }

    public async Task<bool> DeleteCommunityPostAsync(int postId, int communityId, int userId)
    {
        var post = await _repo.GetByIdAsync(postId);
        if (post == null || post.CommunityId != communityId) return false;
        
        if (post.UserId != userId) return false; // Only post owner can delete
        
        return await _repo.DeleteAsync(postId);
    }

    public async Task<bool> IsPostInCommunityAsync(int postId, int communityId)
    {
        var post = await _repo.GetByIdAsync(postId);
        return post?.CommunityId == communityId;
    }

    public async Task<bool> IsUserPostOwnerAsync(int postId, int userId)
    {
        var post = await _repo.GetByIdAsync(postId);
        return post?.UserId == userId;
    }
} 