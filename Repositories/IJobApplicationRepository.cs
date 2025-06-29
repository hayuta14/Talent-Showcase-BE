using TalentShowCase.API.DTOs.Job;
using TalentShowCase.API.Models;

namespace TalentShowCase.API.Repositories;

public interface IJobApplicationRepository
{
    Task<JobApplication> CreateApplicationAsync(JobApplication application);
    Task<JobApplication?> GetApplicationByIdAsync(int applicationId);
    Task<JobApplication?> GetApplicationByJobAndUserAsync(int jobId, int userId);
    Task<List<JobApplication>> GetApplicationsByJobAsync(int jobId);
    Task<List<JobApplication>> GetApplicationsByUserAsync(int userId);
    Task<JobApplication> UpdateApplicationAsync(JobApplication application);
    Task<bool> DeleteApplicationAsync(int applicationId);
    Task<bool> HasUserAppliedToJobAsync(int jobId, int userId);
} 