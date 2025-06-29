using TalentShowCase.API.DTOs.Job;
using TalentShowCase.API.Models;
using TalentShowCase.API.Repositories;

namespace TalentShowCase.API.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly IJobService _jobService;

    public JobApplicationService(IJobApplicationRepository applicationRepository, IJobService jobService)
    {
        _applicationRepository = applicationRepository;
        _jobService = jobService;
    }

    public async Task<JobApplicationResponseDTO> ApplyToJobAsync(JobApplicationRequestDTO dto, int userId)
    {
        // Kiểm tra xem job có tồn tại không
        var job = await _jobService.GetJobByIdAsync(dto.JobId);
        if (job == null)
            throw new Exception("Job không tồn tại");

        // Kiểm tra xem user đã apply job này chưa
        var hasApplied = await _applicationRepository.HasUserAppliedToJobAsync(dto.JobId, userId);
        if (hasApplied)
            throw new Exception("Bạn đã apply job này rồi");

        var application = new JobApplication
        {
            JobId = dto.JobId,
            ApplicantId = userId,
            CoverLetter = dto.CoverLetter,
            ResumeUrl = dto.ResumeUrl,
            Status = ApplicationStatus.Pending,
            AppliedAt = DateTime.UtcNow
        };

        var createdApplication = await _applicationRepository.CreateApplicationAsync(application);
        return MapToResponseDTO(createdApplication);
    }

    public async Task<JobApplicationResponseDTO> GetApplicationByIdAsync(int applicationId, int userId)
    {
        var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
        if (application == null)
            throw new Exception("Application không tồn tại");

        // Kiểm tra quyền truy cập: chỉ applicant hoặc job owner mới được xem
        if (application.ApplicantId != userId && application.Job.UserId != userId)
            throw new Exception("Không có quyền truy cập application này");

        return MapToResponseDTO(application);
    }

    public async Task<List<JobApplicationResponseDTO>> GetMyApplicationsAsync(int userId)
    {
        var applications = await _applicationRepository.GetApplicationsByUserAsync(userId);
        return applications.Select(MapToResponseDTO).ToList();
    }

    public async Task<List<JobApplicationResponseDTO>> GetApplicationsForMyJobAsync(int jobId, int jobOwnerId)
    {
        var applications = await _applicationRepository.GetApplicationsByJobAsync(jobId);
        
        // Kiểm tra xem job có thuộc về user này không
        if (applications.Any() && applications.First().Job.UserId != jobOwnerId)
            throw new Exception("Không có quyền xem applications của job này");

        return applications.Select(MapToResponseDTO).ToList();
    }

    public async Task<JobApplicationResponseDTO> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationStatusDTO dto, int jobOwnerId)
    {
        var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
        if (application == null)
            throw new Exception("Application không tồn tại");

        // Kiểm tra xem user có phải là owner của job không
        if (application.Job.UserId != jobOwnerId)
            throw new Exception("Không có quyền cập nhật status của application này");

        application.Status = dto.Status;
        application.Notes = dto.Notes;
        application.UpdatedAt = DateTime.UtcNow;

        var updatedApplication = await _applicationRepository.UpdateApplicationAsync(application);
        return MapToResponseDTO(updatedApplication);
    }

    public async Task<bool> WithdrawApplicationAsync(int applicationId, int userId)
    {
        var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
        if (application == null)
            throw new Exception("Application không tồn tại");

        // Kiểm tra xem user có phải là applicant không
        if (application.ApplicantId != userId)
            throw new Exception("Không có quyền rút application này");

        application.Status = ApplicationStatus.Withdrawn;
        application.UpdatedAt = DateTime.UtcNow;

        await _applicationRepository.UpdateApplicationAsync(application);
        return true;
    }

    public async Task<bool> HasAppliedToJobAsync(int jobId, int userId)
    {
        return await _applicationRepository.HasUserAppliedToJobAsync(jobId, userId);
    }

    private static JobApplicationResponseDTO MapToResponseDTO(JobApplication application)
    {
        return new JobApplicationResponseDTO
        {
            ApplicationId = application.ApplicationId,
            JobId = application.JobId,
            JobTitle = application.Job.JobTitle,
            CompanyName = application.Job.CompanyName ?? "",
            ApplicantId = application.ApplicantId,
            ApplicantName = application.Applicant.Username,
            ApplicantEmail = application.Applicant.Email,
            Status = application.Status,
            CoverLetter = application.CoverLetter,
            ResumeUrl = application.ResumeUrl,
            AppliedAt = application.AppliedAt,
            UpdatedAt = application.UpdatedAt,
            Notes = application.Notes
        };
    }
} 