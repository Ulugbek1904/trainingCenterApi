using System;
using System.ComponentModel.DataAnnotations;
using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models.DTOs
{
    public class CourseUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public Guid TeacherId { get; set; }
        [Range(1, int.MaxValue)]
        public int MaxStudents { get; set; }
        public string Schedule { get; set; }
        public bool IsActive { get; set; }
        public int? CategoryId { get; set; }
        public string Materials { get; set; }
        public CourseLevel Level { get; set; }
    }
}