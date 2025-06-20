using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace TalentShowCase.API.Services
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);
        Task<VideoUploadResult> UploadVideoAsync(IFormFile file);
        Task<DeletionResult> DeleteMediaAsync(string publicId);
    }
} 