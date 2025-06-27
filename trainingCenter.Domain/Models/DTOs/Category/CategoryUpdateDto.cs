using System.ComponentModel.DataAnnotations;

namespace trainingCenter.Domain.Models.DTOs
{
    public class CategoryUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}