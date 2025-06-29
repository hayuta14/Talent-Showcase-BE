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

    Task<List<Post>> GetPostsByUserIdAsync(int userId);

    Task<Comment?> AddSubCommentAsync(int commentId, SubComment subComment);

    Task<Comment?> UpdateSubCommentAsync(int commentId, int subCommentIndex, string content);
    Task<Comment?> DeleteSubCommentAsync(int commentId, int subCommentIndex);

    Task<(int CommentCount, int SubCommentCount, int TotalCount)> GetCommentAndSubCommentCountAsync(int postId);

    Task<Dictionary<int, TalentShowCase.API.Models.User>> GetUserDictionaryByIds(List<int> userIds);

    // Community post methods
    Task<List<Post>> GetCommunityPostsAsync(int communityId, int page, int pageSize, string? searchTerm = null, int? categoryId = null, string sortBy = "newest");
    Task<List<Post>> GetCommunityPostsByUserAsync(int communityId, int userId);
    Task<int> GetCommunityPostCountAsync(int communityId);
} 