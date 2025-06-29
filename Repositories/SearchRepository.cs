using Microsoft.EntityFrameworkCore;
using TalentShowCase.API.Data;
using TalentShowCase.API.DTOs.Search;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly ApplicationDbContext _context;

    public SearchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<SearchResultDTO> Results, int TotalCount, Dictionary<string, int> TypeCounts)> SearchAsync(
        string? query, 
        string? type, 
        string? level, 
        int? talentId,
        int page, 
        int pageSize, 
        string? sortBy, 
        string? sortOrder,
        int? currentUserId = null)
    {
        var results = new List<SearchResultDTO>();
        var typeCounts = new Dictionary<string, int>();

        // Build search query
        var searchQuery = query?.ToLowerInvariant();
        var searchType = type?.ToLowerInvariant();
        var searchLevel = level?.ToLowerInvariant();

        // Search Users
        if (string.IsNullOrEmpty(searchType) || searchType == "user" || searchType == "all")
        {
            var userQuery = _context.Users.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchQuery))
            {
                userQuery = userQuery.Where(u => 
                    EF.Functions.ILike(u.Username, $"%{searchQuery}%") ||
                    EF.Functions.ILike(u.Email, $"%{searchQuery}%") ||
                    EF.Functions.ILike(u.Bio ?? "", $"%{searchQuery}%"));
            }

            if (talentId.HasValue && !string.IsNullOrEmpty(searchLevel))
            {
                userQuery = userQuery.Where(u =>
                    u.UserTalentCategories.Any(utc => utc.TalentCategoryId == talentId.Value && EF.Functions.ILike(utc.Level, $"%{searchLevel}%")));
            }
            else if (talentId.HasValue)
            {
                userQuery = userQuery.Where(u =>
                    u.UserTalentCategories.Any(utc => utc.TalentCategoryId == talentId.Value));
            }
            else if (!string.IsNullOrEmpty(searchLevel))
            {
                userQuery = userQuery.Where(u =>
                    u.UserTalentCategories.Any(utc => EF.Functions.ILike(utc.Level, $"%{searchLevel}%")));
            }

            var users = await userQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new { u.UserId, u.Username, u.Bio, u.ProfilePictureUrl, u.CreatedAt, u.Email, u.IsActive })
                .ToListAsync();

            var userResults = users.Select(u => new SearchResultDTO
            {
                Type = "user",
                Id = u.UserId,
                Title = u.Username,
                Description = u.Bio,
                ImageUrl = u.ProfilePictureUrl,
                CreatedAt = u.CreatedAt,
                RelevanceScore = 1.0,
                AdditionalData = new Dictionary<string, object>
                {
                    ["email"] = u.Email,
                    ["isActive"] = u.IsActive
                }
            }).ToList();

            results.AddRange(userResults);
            typeCounts["user"] = await userQuery.CountAsync();
        }

        // Search Communities
        if (string.IsNullOrEmpty(searchType) || searchType == "community" || searchType == "all")
        {
            var communityQuery = _context.Communities
                .Include(c => c.Creator)
                .Include(c => c.Members)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                communityQuery = communityQuery.Where(c =>
                    EF.Functions.ILike(c.Name, $"%{searchQuery}%") ||
                    EF.Functions.ILike(c.Description ?? "", $"%{searchQuery}%"));
            }

            var communities = await communityQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new { 
                    c.CommunityId, 
                    c.Name, 
                    c.Description, 
                    c.CreatedAt, 
                    c.CreatorId,
                    CreatorName = c.Creator.Username, 
                    MemberCount = c.Members.Count,
                    Members = c.Members
                })
                .ToListAsync();

            var communityResults = communities.Select(c => {
                var additionalData = new Dictionary<string, object>
                {
                    ["creatorName"] = c.CreatorName,
                    ["memberCount"] = c.MemberCount
                };

                // Add membership and creator status if currentUserId is provided
                if (currentUserId.HasValue)
                {
                    var isMember = c.Members.Any(m => m.UserId == currentUserId.Value);
                    var isCreator = c.CreatorId == currentUserId.Value;
                    var memberRole = c.Members.FirstOrDefault(m => m.UserId == currentUserId.Value)?.Role;

                    additionalData["isMember"] = isMember;
                    additionalData["isCreator"] = isCreator;
                    additionalData["memberRole"] = memberRole ?? "";
                }

                return new SearchResultDTO
                {
                    Type = "community",
                    Id = c.CommunityId,
                    Title = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    RelevanceScore = 1.0,
                    AdditionalData = additionalData
                };
            }).ToList();

            results.AddRange(communityResults);
            typeCounts["community"] = await communityQuery.CountAsync();
        }

        // Search Jobs
        if (string.IsNullOrEmpty(searchType) || searchType == "job" || searchType == "all")
        {
            var jobQuery = _context.Jobs
                .Include(j => j.User)
                .Include(j => j.JobTalentCategories)
                .ThenInclude(jtc => jtc.TalentCategory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                jobQuery = jobQuery.Where(j =>
                    EF.Functions.ILike(j.JobTitle, $"%{searchQuery}%") ||
                    EF.Functions.ILike(j.JobDescription ?? "", $"%{searchQuery}%"));
            }

            if (!string.IsNullOrEmpty(searchLevel))
            {
                jobQuery = jobQuery.Where(j => 
                    j.JobTalentCategories.Any(jtc => 
                        EF.Functions.ILike(jtc.Level, $"%{searchLevel}%")));
            }

            if (talentId.HasValue)
            {
                jobQuery = jobQuery.Where(j =>
                    j.JobTalentCategories.Any(jtc => jtc.TalentCategoryId == talentId.Value));
            }

            var jobs = await jobQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new { j.JobId, j.JobTitle, j.JobDescription, j.CompanyName, j.Location, j.Salary, EmployerName = j.User.Username })
                .ToListAsync();

            var jobResults = jobs.Select(j => new SearchResultDTO
            {
                Type = "job",
                Id = j.JobId,
                Title = j.JobTitle,
                Description = j.JobDescription,
                CreatedAt = DateTime.UtcNow, // Jobs don't have CreatedAt
                RelevanceScore = 1.0,
                AdditionalData = new Dictionary<string, object>
                {
                    ["employerName"] = j.EmployerName,
                    ["companyName"] = j.CompanyName ?? "",
                    ["location"] = j.Location ?? "",
                    ["salary"] = j.Salary ?? ""
                }
            }).ToList();

            results.AddRange(jobResults);
            typeCounts["job"] = await jobQuery.CountAsync();
        }

        // Search Talent Categories
        if (string.IsNullOrEmpty(searchType) || searchType == "talent" || searchType == "all")
        {
            var talentQuery = _context.TalentCategories.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                talentQuery = talentQuery.Where(t =>
                    EF.Functions.ILike(t.Name, $"%{searchQuery}%") ||
                    EF.Functions.ILike(t.Description ?? "", $"%{searchQuery}%"));
            }
            if (talentId.HasValue)
            {
                talentQuery = talentQuery.Where(t => t.CategoryId == talentId.Value);
            }

            var talents = await talentQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new { t.CategoryId, t.Name, t.Description, UserCount = t.UserTalentCategories.Count })
                .ToListAsync();

            var talentResults = talents.Select(t => new SearchResultDTO
            {
                Type = "talent",
                Id = t.CategoryId,
                Title = t.Name,
                Description = t.Description,
                CreatedAt = DateTime.UtcNow, // Talent categories don't have CreatedAt
                RelevanceScore = 1.0,
                AdditionalData = new Dictionary<string, object>
                {
                    ["userCount"] = t.UserCount
                }
            }).ToList();

            results.AddRange(talentResults);
            typeCounts["talent"] = await talentQuery.CountAsync();
        }

        // Search Posts
        if (string.IsNullOrEmpty(searchType) || searchType == "post" || searchType == "all")
        {
            var postQuery = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                postQuery = postQuery.Where(p =>
                    EF.Functions.ILike(p.Description, $"%{searchQuery}%"));
            }
            if (talentId.HasValue)
            {
                postQuery = postQuery.Where(p => p.CategoryId == talentId.Value);
            }

            var posts = await postQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new { p.PostId, p.Description, p.VideoUrl, p.UploadedAt, p.IsPublic, AuthorName = p.User.Username, CategoryName = p.Category != null ? p.Category.Name : "", LikeCount = p.Likes.Count, CommentCount = p.Comments.Count })
                .ToListAsync();

            var postResults = posts.Select(p => new SearchResultDTO
            {
                Type = "post",
                Id = p.PostId,
                Title = $"Post by {p.AuthorName}",
                Description = p.Description,
                ImageUrl = p.VideoUrl, // Use video URL as image for posts
                CreatedAt = p.UploadedAt,
                RelevanceScore = 1.0,
                AdditionalData = new Dictionary<string, object>
                {
                    ["authorName"] = p.AuthorName,
                    ["categoryName"] = p.CategoryName,
                    ["likeCount"] = p.LikeCount,
                    ["commentCount"] = p.CommentCount,
                    ["isPublic"] = p.IsPublic
                }
            }).ToList();

            results.AddRange(postResults);
            typeCounts["post"] = await postQuery.CountAsync();
        }

        // Sort results
        var sortedResults = sortBy?.ToLowerInvariant() switch
        {
            "date" => sortOrder?.ToLowerInvariant() == "desc" 
                ? results.OrderByDescending(r => r.CreatedAt).ToList()
                : results.OrderBy(r => r.CreatedAt).ToList(),
            "name" => sortOrder?.ToLowerInvariant() == "desc"
                ? results.OrderByDescending(r => r.Title).ToList()
                : results.OrderBy(r => r.Title).ToList(),
            _ => results.OrderByDescending(r => r.RelevanceScore).ToList()
        };

        var totalCount = typeCounts.Values.Sum();

        return (sortedResults, totalCount, typeCounts);
    }

    public async Task<SearchFilterDTO> GetAvailableFiltersAsync()
    {
        var filters = new SearchFilterDTO
        {
            Types = new List<string> { "user", "community", "job", "talent", "post" },
            Levels = await _context.JobTalentCategories
                .Select(jtc => jtc.Level)
                .Distinct()
                .ToListAsync()
        };

        return filters;
    }
} 