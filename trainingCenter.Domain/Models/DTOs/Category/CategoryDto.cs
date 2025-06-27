using System.Collections.Generic;

namespace trainingCenter.Domain.Models.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Course> Courses { get; set; }
    }
}