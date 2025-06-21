using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public interface IPostRepository
{
    Task<Post> CreateAsync(Post post);
    Task<Post?> GetByIdAsync(int id);
    Task<Post> UpdateAsync(Post post);
    Task<bool> DeleteAsync(int id);
    Task<(List<Post> Items, int TotalItems)> GetAllAsync(int page, int pageSize);
    
    // Like methods
    Task<PostLike> CreateLikeAsync(PostLike like);
    Task<bool> DeleteLikeAsync(int postId, int userId);
    Task<bool> ExistsLikeAsync(int postId, int userId);
    Task<int> GetLikeCountAsync(int postId);
    
    // Comment methods
    Task<Comment> CreateCommentAsync(Comment comment);
    Task<(List<Comment> Items, int TotalItems)> GetCommentsByPostIdAsync(int postId, int page, int pageSize);
    Task<Comment?> GetCommentByIdAsync(int commentId);
    Task<Comment> UpdateCommentAsync(Comment comment);
    Task<bool> DeleteCommentAsync(int commentId);
    Task<int> GetCommentCountAsync(int postId);
} 