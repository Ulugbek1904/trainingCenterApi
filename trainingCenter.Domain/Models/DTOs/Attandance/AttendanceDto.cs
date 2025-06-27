using System;
using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models.DTOs
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime Date { get; set; }
        public Status Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}