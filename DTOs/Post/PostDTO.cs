using System.ComponentModel.DataAnnotations;

namespace TalentShowCase.API.DTOs.Post
{
    public class PostDTO
    {   
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public string VideoUrl { get; set; } = string.Empty;


        [Required]
        public bool IsPublic { get; set; }

    }
}