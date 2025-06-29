using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using TalentShowCase.API.Models;
using Microsoft.Extensions.Logging;

namespace TalentShowCase.API.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ILogger<ApplicationDbContext>? _logger;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ILogger<ApplicationDbContext>? logger = null)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Follower> Followers { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<PostLike> PostLikes { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Community> Communities { get; set; } = null!;
    public DbSet<CommunityMember> CommunityMembers { get; set; } = null!;
    public DbSet<TalentCategory> TalentCategories { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserTalentCategory> UserTalentCategories { get; set; } = null!;
    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<JobTalentCategory> JobTalentCategories { get; set; } = null!;
    public DbSet<JobApplication> JobApplications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Role configurations
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();

        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, Name = "Admin", Description = "Administrator with full access" },
            new Role { RoleId = 2, Name = "User", Description = "Regular user with standard access" }
        );

        // User configurations
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Set default role to User (RoleId = 2)
        modelBuilder.Entity<User>()
            .Property(u => u.RoleId)
            .HasDefaultValue(2);

        // RefreshToken configurations
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Follower configurations
        modelBuilder.Entity<Follower>()
            .HasKey(f => new { f.FollowerId, f.FollowedId });

        modelBuilder.Entity<Follower>()
            .HasOne(f => f.FollowerUser)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Follower>()
            .HasOne(f => f.FollowedUser)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowedId)
            .OnDelete(DeleteBehavior.Cascade);

        // Message configurations
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Cascade);

        // Post configurations
        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.SharedFromPost)
            .WithMany()
            .HasForeignKey(p => p.SharedFromPostId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.Community)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CommunityId)
            .OnDelete(DeleteBehavior.SetNull);

        // PostLike configurations
        modelBuilder.Entity<PostLike>()
            .HasOne(pl => pl.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(pl => pl.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PostLike>()
            .HasOne(pl => pl.User)
            .WithMany(u => u.PostLikes)
            .HasForeignKey(pl => pl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment configurations
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình SubComments lưu dạng jsonb với ValueConverter
        var subCommentConverter = new ValueConverter<List<SubComment>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<List<SubComment>>(v, (JsonSerializerOptions)null) ?? new List<SubComment>()
        );
        modelBuilder.Entity<Comment>()
            .Property(c => c.SubComments)
            .HasConversion(subCommentConverter)
            .HasColumnType("jsonb");

        // Community configurations
        modelBuilder.Entity<Community>()
            .HasOne(c => c.Creator)
            .WithMany(u => u.CreatedCommunities)
            .HasForeignKey(c => c.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // CommunityMember configurations
        modelBuilder.Entity<CommunityMember>()
            .HasKey(cm => new { cm.CommunityId, cm.UserId });

        modelBuilder.Entity<CommunityMember>()
            .HasOne(cm => cm.Community)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.CommunityId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommunityMember>()
            .HasOne(cm => cm.User)
            .WithMany(u => u.CommunityMemberships)
            .HasForeignKey(cm => cm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-many: User <-> TalentCategory với trường level
        modelBuilder.Entity<UserTalentCategory>()
            .HasKey(utc => new { utc.UserId, utc.TalentCategoryId });
        modelBuilder.Entity<UserTalentCategory>()
            .HasOne(utc => utc.User)
            .WithMany(u => u.UserTalentCategories)
            .HasForeignKey(utc => utc.UserId);
        modelBuilder.Entity<UserTalentCategory>()
            .HasOne(utc => utc.TalentCategory)
            .WithMany(tc => tc.UserTalentCategories)
            .HasForeignKey(utc => utc.TalentCategoryId);

        // Many-to-many: Job <-> TalentCategory
        modelBuilder.Entity<JobTalentCategory>()
            .HasKey(jtc => new { jtc.JobId, jtc.TalentCategoryId });
        modelBuilder.Entity<JobTalentCategory>()
            .HasOne(jtc => jtc.Job)
            .WithMany(j => j.JobTalentCategories)
            .HasForeignKey(jtc => jtc.JobId);
        modelBuilder.Entity<JobTalentCategory>()
            .HasOne(jtc => jtc.TalentCategory)
            .WithMany(tc => tc.JobTalentCategories)
            .HasForeignKey(jtc => jtc.TalentCategoryId);

        // JobApplication configurations
        modelBuilder.Entity<JobApplication>()
            .HasOne(ja => ja.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(ja => ja.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobApplication>()
            .HasOne(ja => ja.Applicant)
            .WithMany(u => u.JobApplications)
            .HasForeignKey(ja => ja.ApplicantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobApplication>()
            .HasIndex(ja => new { ja.JobId, ja.ApplicantId })
            .IsUnique();
    }
} 