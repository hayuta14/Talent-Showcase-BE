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
        var query = _context.Posts.AsQueryable();
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
        var query = _context.Comments.Where(c => c.PostId == postId);
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
} 