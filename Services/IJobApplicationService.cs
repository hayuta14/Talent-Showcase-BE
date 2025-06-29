using TalentShowCase.API.DTOs.Job;

namespace TalentShowCase.API.Services;

public interface IJobApplicationService
{
    Task<JobApplicationResponseDTO> ApplyToJobAsync(JobApplicationRequestDTO dto, int userId);
    Task<JobApplicationResponseDTO> GetApplicationByIdAsync(int applicationId, int userId);
    Task<List<JobApplicationResponseDTO>> GetMyApplicationsAsync(int userId);
    Task<List<JobApplicationResponseDTO>> GetApplicationsForMyJobAsync(int jobId, int jobOwnerId);
    Task<JobApplicationResponseDTO> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationStatusDTO dto, int jobOwnerId);
    Task<bool> WithdrawApplicationAsync(int applicationId, int userId);
    Task<bool> HasAppliedToJobAsync(int jobId, int userId);
} 