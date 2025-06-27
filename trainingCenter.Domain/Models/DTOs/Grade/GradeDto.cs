using System;

namespace trainingCenter.Domain.Models.DTOs
{
    public class GradeDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; }
    }
}