using Microsoft.AspNetCore.Mvc;
using TalentShowCase.API.Services;

namespace TalentShowCase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public MediaController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var result = await _cloudinaryService.UploadImageAsync(file);
            return Ok(new
            {
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId
            });
        }

        [HttpPost("upload-video")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var result = await _cloudinaryService.UploadVideoAsync(file);
            return Ok(new
            {
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId
            });
        }

        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteMedia(string publicId)
        {
            var result = await _cloudinaryService.DeleteMediaAsync(publicId);
            return Ok(result);
        }
    }
} 