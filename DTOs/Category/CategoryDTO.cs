using System.ComponentModel.DataAnnotations;

namespace TalentShowCase.API.DTOs.Category
{
    public class CategoryDTO
    {   
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
    }
}