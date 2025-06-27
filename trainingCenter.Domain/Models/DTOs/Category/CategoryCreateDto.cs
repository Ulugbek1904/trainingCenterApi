using System.ComponentModel.DataAnnotations;

namespace trainingCenter.Domain.Models.DTOs
{
    public class CategoryCreateDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}