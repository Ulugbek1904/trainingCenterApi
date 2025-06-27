using System;
using System.ComponentModel.DataAnnotations;

namespace trainingCenter.Domain.Models.DTOs
{
    public class StudentCourseCreateDto
    {
        [Required]
        public Guid StudentId { get; set; }
        [Required]
        public Guid CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        [StringLength(100)]
        public string GroupName { get; set; }
        public bool IsActive { get; set; } = true;
        [Range(0, double.MaxValue)]
        public decimal? Discount { get; set; }
        [StringLength(1000)]
        public string Notes { get; set; }
    }
}