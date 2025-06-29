using TalentShowCase.API.DTOs.Job;

namespace TalentShowCase.API.Services;

public interface IJobService
{
    Task<JobResponseDTO> CreateJobAsync(JobRequestDTO dto, int userId);
    Task<JobResponseDTO> UpdateJobAsync(int jobId, JobRequestDTO dto, int userId);
    Task<bool> DeleteJobAsync(int jobId, int userId);
    Task<List<JobResponseDTO>> GetJobsByUserTalentAsync(int userId);
    Task<JobResponseDTO?> GetJobByIdAsync(int jobId);
    Task<JobResponseDTO?> GetJobDetailAsync(int jobId, int? userId = null);
} 