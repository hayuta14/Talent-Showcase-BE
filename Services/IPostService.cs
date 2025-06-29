using System.Security.Claims;
using TalentShowCase.API.DTOs.Post;

namespace TalentShowCase.API.Services;

public interface IPostService
{
    Task<PostResponseDTO> CreateAsync(PostDTO dto, int userId);
    Task<PostResponseDTO?> GetByIdAsync(int id);
    Task<PostResponseDTO> UpdateAsync(int id, PostDTO dto, int userId);
    Task<bool> DeleteAsync(int id, int userId);
    Task<(List<PostResponseDTO> Items, object Metadata)> GetAllAsync(int page, int pageSize, int userId);
    
    // Like methods
    Task<(bool Success, string? Error)> LikePostAsync(PostLikeDTO dto, int userId);
    Task<int> GetLikeCountAsync(int postId);
    
    // Comment methods
    Task<(bool Success, string? Error, CommentResponseDTO? Comment)> CreateCommentAsync(CommentDTO dto, int userId);
    Task<(bool Success, string? Error, object? Data, object? Metadata)> GetCommentsByPostIdAsync(int postId, int page, int pageSize);
    Task<(bool Success, string? Error, CommentResponseDTO? Comment)> UpdateCommentAsync(int commentId, CommentUpdateDTO dto, int userId);
    Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId, int userId);
    Task<int> GetCommentCountAsync(int postId);

    Task<List<PostResponseDTO>> GetPostsByUserIdAsync(int userId);

    Task<(bool Success, string? Error, CommentResponseDTO? Comment)> AddSubCommentAsync(int commentId, int userId, string content);

    Task<(bool Success, string? Error, CommentResponseDTO? Comment)> UpdateSubCommentAsync(int commentId, int subCommentIndex, int userId, string content);
    Task<(bool Success, string? Error, CommentResponseDTO? Comment)> DeleteSubCommentAsync(int commentId, int subCommentIndex, int userId);

    Task<(int CommentCount, int SubCommentCount, int TotalCount)> GetCommentAndSubCommentCountAsync(int postId);

    // Community post methods
    Task<PostResponseDTO> CreateCommunityPostAsync(PostDTO dto, int communityId, int userId);
    Task<PostResponseDTO?> GetCommunityPostByIdAsync(int postId, int communityId, int userId);
    Task<List<PostResponseDTO>> GetCommunityPostsAsync(int communityId, int page, int pageSize, int userId, string? searchTerm = null, int? categoryId = null, string sortBy = "newest");
    Task<List<PostResponseDTO>> GetCommunityPostsByUserAsync(int communityId, int targetUserId, int currentUserId);
    Task<PostResponseDTO> UpdateCommunityPostAsync(int postId, PostDTO dto, int communityId, int userId);
    Task<bool> DeleteCommunityPostAsync(int postId, int communityId, int userId);
    Task<bool> IsPostInCommunityAsync(int postId, int communityId);
    Task<bool> IsUserPostOwnerAsync(int postId, int userId);
} 