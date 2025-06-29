using Microsoft.EntityFrameworkCore;
using TalentShowCase.API.Data;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;
    public PostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Post> CreateAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _context.Posts.FindAsync(id);
    }

    public async Task<Post> UpdateAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null) return false;
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(List<Post> Items, int TotalItems)> GetAllAsync(int page, int pageSize)
    {
        var query = _context.Posts.Where(p => p.IsPublic)
            .Include(p => p.User)
            .OrderByDescending(p => p.UploadedAt)
            .AsQueryable();
        var totalItems = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalItems);
    }

    public async Task<bool> ExistsLikeAsync(int postId, int userId)
    {
        return await _context.PostLikes.AnyAsync(pl => pl.PostId == postId && pl.UserId == userId);
    }

    public async Task<PostLike> CreateLikeAsync(PostLike like)
    {
        _context.PostLikes.Add(like);
        await _context.SaveChangesAsync();
        return like;
    }

    public async Task<bool> DeleteLikeAsync(int postId, int userId)
    {
        var like = await _context.PostLikes.FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);
        if (like == null) return false;
        _context.PostLikes.Remove(like);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetLikeCountAsync(int postId)
    {
        return await _context.PostLikes.CountAsync(pl => pl.PostId == postId);
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<(List<Comment> Items, int TotalItems)> GetCommentsByPostIdAsync(int postId, int page, int pageSize)
    {
        var query = _context.Comments.Where(c => c.PostId == postId).Include(c => c.User);
        var totalItems = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalItems);
    }

    public async Task<Comment?> GetCommentByIdAsync(int commentId)
    {
        return await _context.Comments.FindAsync(commentId);
    }

    public async Task<Comment> UpdateCommentAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> DeleteCommentAsync(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null) return false;
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetCommentCountAsync(int postId)
    {
        return await _context.Comments.CountAsync(c => c.PostId == postId);
    }

    public async Task<List<Post>> GetPostsByUserIdAsync(int userId)
    {
        return await _context.Posts
            .Where(p => p.UserId == userId && p.IsPublic)
            .Include(p => p.User)
            .OrderByDescending(p => p.UploadedAt)
            .ToListAsync();
    }

    public async Task<Comment?> AddSubCommentAsync(int commentId, SubComment subComment)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null) return null;
        if (comment.SubComments == null)
            comment.SubComments = new List<SubComment>();
        comment.SubComments.Add(subComment);
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> UpdateSubCommentAsync(int commentId, int subCommentIndex, string content)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null || comment.SubComments == null || subCommentIndex < 0 || subCommentIndex >= comment.SubComments.Count)
            return null;
        comment.SubComments[subCommentIndex].Content = content;
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> DeleteSubCommentAsync(int commentId, int subCommentIndex)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null || comment.SubComments == null || subCommentIndex < 0 || subCommentIndex >= comment.SubComments.Count)
            return null;
        comment.SubComments.RemoveAt(subCommentIndex);
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<(int CommentCount, int SubCommentCount, int TotalCount)> GetCommentAndSubCommentCountAsync(int postId)
    {
        var comments = await _context.Comments.Where(c => c.PostId == postId).ToListAsync();
        int commentCount = comments.Count;
        int subCommentCount = 0;
        foreach (var c in comments)
        {
            int count = c.SubComments?.Count ?? 0;
            Console.WriteLine($"CommentId: {c.CommentId}, SubCommentCount: {count}");
            subCommentCount += count;
        }
        int total = commentCount + subCommentCount;
        Console.WriteLine($"Total comment: {commentCount}, total subcomment: {subCommentCount}, total: {total}");
        return (commentCount, subCommentCount, total);
    }

    public async Task<Dictionary<int, User>> GetUserDictionaryByIds(List<int> userIds)
    {
        if (userIds == null || userIds.Count == 0) return new Dictionary<int, User>();
        var users = await _context.Users.Where(u => userIds.Contains(u.UserId)).ToListAsync();
        return users.ToDictionary(u => u.UserId, u => u);
    }

    // Community post methods
    public async Task<List<Post>> GetCommunityPostsAsync(int communityId, int page, int pageSize, string? searchTerm = null, int? categoryId = null, string sortBy = "newest")
    {
        var query = _context.Posts
            .Where(p => p.CommunityId == communityId)
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Description.Contains(searchTerm));
        }

        // Apply category filter
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "oldest" => query.OrderBy(p => p.UploadedAt),
            "mostLiked" => query.OrderByDescending(p => p.Likes.Count),
            "mostCommented" => query.OrderByDescending(p => p.Comments.Count),
            _ => query.OrderByDescending(p => p.UploadedAt) // newest
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Post>> GetCommunityPostsByUserAsync(int communityId, int userId)
    {
        return await _context.Posts
            .Where(p => p.CommunityId == communityId && p.UserId == userId)
            .Include(p => p.User)
            .Include(p => p.Category)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.UploadedAt)
            .ToListAsync();
    }

    public async Task<int> GetCommunityPostCountAsync(int communityId)
    {
        return await _context.Posts
            .CountAsync(p => p.CommunityId == communityId);
    }
} 