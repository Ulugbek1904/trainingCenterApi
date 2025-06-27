using System;
using System.ComponentModel.DataAnnotations;
using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models.DTOs
{
    public class AttendanceUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid StudentId { get; set; }
        [Required]
        public Guid CourseId { get; set; }
        public DateTime Date { get; set; }
        public Status Status { get; set; }
        [StringLength(1000)]
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}