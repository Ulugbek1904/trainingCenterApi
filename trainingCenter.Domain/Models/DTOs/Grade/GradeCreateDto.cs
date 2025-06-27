using System;
using System.ComponentModel.DataAnnotations;

namespace trainingCenter.Domain.Models.DTOs
{
    public class GradeCreateDto
    {
        [Required]
        public Guid StudentId { get; set; }
        [Required]
        public Guid CourseId { get; set; }
        [Range(0, 100)]
        public int Score { get; set; }
        [StringLength(1000)]
        public string Comment { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [Required]
        public Guid TeacherId { get; set; }
    }
}