using System.ComponentModel.DataAnnotations;

namespace TalentShowCase.API.DTOs.Post
{
    public class PostDTO
    {   
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Description { get; set; }

        public string VideoUrl { get; set; }

        [Required]
        public bool IsPublic { get; set; }

    }
}