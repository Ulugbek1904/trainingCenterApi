using System;

namespace trainingCenter.Domain.Models.DTOs
{
    public class StudentCourseDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
        public decimal? Discount { get; set; }
        public string Notes { get; set; }
    }
}