using TalentShowCase.API.DTOs.Job;
using TalentShowCase.API.Models;
using TalentShowCase.API.Data;
using Microsoft.EntityFrameworkCore;
using TalentShowCase.API.DTOs.Common;

namespace TalentShowCase.API.Services;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _context;
    public JobService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobResponseDTO> CreateJobAsync(JobRequestDTO dto, int userId)
    {
        try
        {
            if (dto.TalentCategoryIds.Count != dto.Levels.Count)
                throw new Exception("TalentCategoryIds và Levels phải có cùng số lượng phần tử");
            var job = new Job
            {
                JobTitle = dto.JobTitle,
                CompanyName = dto.CompanyName,
                Location = dto.Location,
                Salary = dto.Salary,
                JobDescription = dto.JobDescription,
                Requirements = dto.Requirements,
                ExpireAt = dto.ExpireAt?.ToUniversalTime(),
                UserId = userId
            };
            for (int i = 0; i < dto.TalentCategoryIds.Count; i++)
            {
                job.JobTalentCategories.Add(new JobTalentCategory
                {
                    TalentCategoryId = dto.TalentCategoryIds[i],
                    Level = dto.Levels[i]
                });
            }
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            var user = await _context.Users.FindAsync(userId);
            return new JobResponseDTO
            {
                JobId = job.JobId,
                JobTitle = job.JobTitle,
                CompanyName = job.CompanyName,
                Location = job.Location,
                Salary = job.Salary,
                JobDescription = job.JobDescription,
                Requirements = job.Requirements,
                ExpireAt = job.ExpireAt,
                TalentCategories = job.JobTalentCategories.Select(jtc => new TalentCategoryLevelDTO
                {
                    Id = jtc.TalentCategoryId,
                    Level = jtc.Level
                }).ToList(),
                UserId = userId,
                UserName = user?.Username
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Create job failed: {ex.Message}");
        }
    }

    public async Task<JobResponseDTO> UpdateJobAsync(int jobId, JobRequestDTO dto, int userId)
    {
        try
        {
            if (dto.TalentCategoryIds.Count != dto.Levels.Count)
                throw new Exception("TalentCategoryIds và Levels phải có cùng số lượng phần tử");
            var job = await _context.Jobs
                .Include(j => j.JobTalentCategories)
                .FirstOrDefaultAsync(j => j.JobId == jobId);
            if (job == null) throw new Exception("Job not found");
            job.JobTitle = dto.JobTitle;
            job.CompanyName = dto.CompanyName;
            job.Location = dto.Location;
            job.Salary = dto.Salary;
            job.JobDescription = dto.JobDescription;
            job.Requirements = dto.Requirements;
            job.ExpireAt = dto.ExpireAt?.ToUniversalTime();
            job.JobTalentCategories.Clear();
            for (int i = 0; i < dto.TalentCategoryIds.Count; i++)
            {
                job.JobTalentCategories.Add(new JobTalentCategory
                {
                    JobId = job.JobId,
                    TalentCategoryId = dto.TalentCategoryIds[i],
                    Level = dto.Levels[i]
                });
            }
            await _context.SaveChangesAsync();
            var user = await _context.Users.FindAsync(job.UserId);
            return new JobResponseDTO
            {
                JobId = job.JobId,
                JobTitle = job.JobTitle,
                CompanyName = job.CompanyName,
                Location = job.Location,
                Salary = job.Salary,
                JobDescription = job.JobDescription,
                Requirements = job.Requirements,
                ExpireAt = job.ExpireAt,
                TalentCategories = job.JobTalentCategories.Select(jtc => new TalentCategoryLevelDTO
                {
                    Id = jtc.TalentCategoryId,
                    Level = jtc.Level
                }).ToList(),
                UserId = job.UserId,
                UserName = user?.Username
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Update job failed: {ex.Message}");
        }
    }

    public async Task<bool> DeleteJobAsync(int jobId, int userId)
    {
        try
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null) throw new Exception("Job not found");
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Delete job failed: {ex.Message}");
        }
    }

    public async Task<List<JobResponseDTO>> GetJobsByUserTalentAsync(int userId)
    {
        try
        {
            var userTalent = await _context.UserTalentCategories
                .Where(utc => utc.UserId == userId)
                .Select(utc => new { utc.TalentCategoryId, utc.Level })
                .ToListAsync();
            var jobs = await _context.Jobs
                .Include(j => j.JobTalentCategories)
                .ThenInclude(jtc => jtc.TalentCategory)
                .Include(j => j.User)
                .ToListAsync();
            var result = jobs
                .Select(j => new
                {
                    Job = j,
                    MatchCount = j.JobTalentCategories.Count(jtc => userTalent.Any(ut => ut.TalentCategoryId == jtc.TalentCategoryId && ut.Level == jtc.Level))
                })
                .OrderByDescending(x => x.MatchCount)
                .ThenByDescending(x => x.Job.JobId)
                .Select(x => new JobResponseDTO
                {
                    JobId = x.Job.JobId,
                    JobTitle = x.Job.JobTitle,
                    CompanyName = x.Job.CompanyName,
                    Location = x.Job.Location,
                    Salary = x.Job.Salary,
                    JobDescription = x.Job.JobDescription,
                    Requirements = x.Job.Requirements,
                    ExpireAt = x.Job.ExpireAt,
                    TalentCategories = x.Job.JobTalentCategories.Select(jtc => new TalentCategoryLevelDTO
                    {
                        Id = jtc.TalentCategoryId,
                        Level = jtc.Level
                    }).ToList(),
                    UserId = x.Job.UserId,
                    UserName = x.Job.User?.Username
                })
                .ToList();
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Get jobs failed: {ex.Message}");
        }
    }

    public async Task<JobResponseDTO?> GetJobByIdAsync(int jobId)
    {
        try
        {
            var job = await _context.Jobs
                .Include(j => j.JobTalentCategories)
                .ThenInclude(jtc => jtc.TalentCategory)
                .Include(j => j.User)
                .FirstOrDefaultAsync(j => j.JobId == jobId);

            if (job == null) return null;

            return new JobResponseDTO
            {
                JobId = job.JobId,
                JobTitle = job.JobTitle,
                CompanyName = job.CompanyName,
                Location = job.Location,
                Salary = job.Salary,
                JobDescription = job.JobDescription,
                Requirements = job.Requirements,
                ExpireAt = job.ExpireAt,
                TalentCategories = job.JobTalentCategories.Select(jtc => new TalentCategoryLevelDTO
                {
                    Id = jtc.TalentCategoryId,
                    Level = jtc.Level
                }).ToList(),
                UserId = job.UserId,
                UserName = job.User?.Username
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Get job by id failed: {ex.Message}");
        }
    }

    public async Task<JobResponseDTO?> GetJobDetailAsync(int jobId, int? userId = null)
    {
        var job = await _context.Jobs
            .Include(j => j.JobTalentCategories)
                .ThenInclude(jtc => jtc.TalentCategory)
            .Include(j => j.User)
            .FirstOrDefaultAsync(j => j.JobId == jobId);
        if (job == null) return null;
        return new JobResponseDTO
        {
            JobId = job.JobId,
            JobTitle = job.JobTitle,
            CompanyName = job.CompanyName,
            Location = job.Location,
            Salary = job.Salary,
            JobDescription = job.JobDescription,
            Requirements = job.Requirements,
            ExpireAt = job.ExpireAt,
            TalentCategories = job.JobTalentCategories.Select(jtc => new TalentCategoryLevelDTO
            {
                Id = jtc.TalentCategoryId,
                Level = jtc.Level
            }).ToList(),
            UserId = job.UserId,
            UserName = job.User?.Username
        };
    }
} 