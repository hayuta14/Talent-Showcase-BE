using System.Security.Claims;
using TalentShowCase.API.DTOs.Post;

namespace TalentShowCase.API.Services;

public interface IPostService
{
    Task<PostResponseDTO> CreateAsync(PostDTO dto, int userId);
    Task<PostResponseDTO?> GetByIdAsync(int id);
    Task<PostResponseDTO> UpdateAsync(int id, PostDTO dto, int userId);
    Task<bool> DeleteAsync(int id, int userId);
    Task<(List<PostResponseDTO> Items, object Metadata)> GetAllAsync(int page, int pageSize);
    
    // Like methods
    Task<(bool Success, string? Error)> LikePostAsync(PostLikeDTO dto, int userId);
    Task<int> GetLikeCountAsync(int postId);
    
    // Comment methods
    Task<(bool Success, string? Error, CommentResponseDTO? Comment)> CreateCommentAsync(CommentDTO dto, int userId);
    Task<(bool Success, string? Error, object? Data, object? Metadata)> GetCommentsByPostIdAsync(int postId, int page, int pageSize);
    Task<(bool Success, string? Error, CommentResponseDTO? Comment)> UpdateCommentAsync(int commentId, CommentUpdateDTO dto, int userId);
    Task<(bool Success, string? Error)> DeleteCommentAsync(int commentId, int userId);
    Task<int> GetCommentCountAsync(int postId);
} 