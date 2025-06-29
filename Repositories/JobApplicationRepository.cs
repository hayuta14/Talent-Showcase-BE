using Microsoft.EntityFrameworkCore;
using TalentShowCase.API.Data;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public JobApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobApplication> CreateApplicationAsync(JobApplication application)
    {
        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.JobApplications
            .Include(ja => ja.Job)
            .Include(ja => ja.Applicant)
            .FirstOrDefaultAsync(ja => ja.ApplicationId == application.ApplicationId) ?? application;
    }

    public async Task<JobApplication?> GetApplicationByIdAsync(int applicationId)
    {
        return await _context.JobApplications
            .Include(ja => ja.Job)
            .Include(ja => ja.Applicant)
            .FirstOrDefaultAsync(ja => ja.ApplicationId == applicationId);
    }

    public async Task<JobApplication?> GetApplicationByJobAndUserAsync(int jobId, int userId)
    {
        return await _context.JobApplications
            .Include(ja => ja.Job)
            .Include(ja => ja.Applicant)
            .FirstOrDefaultAsync(ja => ja.JobId == jobId && ja.ApplicantId == userId);
    }

    public async Task<List<JobApplication>> GetApplicationsByJobAsync(int jobId)
    {
        return await _context.JobApplications
            .Include(ja => ja.Job)
            .Include(ja => ja.Applicant)
            .Where(ja => ja.JobId == jobId)
            .OrderByDescending(ja => ja.AppliedAt)
            .ToListAsync();
    }

    public async Task<List<JobApplication>> GetApplicationsByUserAsync(int userId)
    {
        return await _context.JobApplications
            .Include(ja => ja.Job)
            .Include(ja => ja.Applicant)
            .Where(ja => ja.ApplicantId == userId)
            .OrderByDescending(ja => ja.AppliedAt)
            .ToListAsync();
    }

    public async Task<JobApplication> UpdateApplicationAsync(JobApplication application)
    {
        application.UpdatedAt = DateTime.UtcNow;
        _context.JobApplications.Update(application);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.JobApplications
            .Include(ja => ja.Job)
            .Include(ja => ja.Applicant)
            .FirstOrDefaultAsync(ja => ja.ApplicationId == application.ApplicationId) ?? application;
    }

    public async Task<bool> DeleteApplicationAsync(int applicationId)
    {
        var application = await _context.JobApplications.FindAsync(applicationId);
        if (application == null) return false;
        
        _context.JobApplications.Remove(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasUserAppliedToJobAsync(int jobId, int userId)
    {
        return await _context.JobApplications
            .AnyAsync(ja => ja.JobId == jobId && ja.ApplicantId == userId);
    }
} 