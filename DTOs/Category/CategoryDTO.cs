using System.ComponentModel.DataAnnotations;

namespace TalentShowCase.API.DTOs.Category
{
    public class CategoryDTO
    {   
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}