using System;
using System.Collections.Generic;
using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int MaxStudents { get; set; }
        public string Schedule { get; set; }
        public bool IsActive { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Materials { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }
        public CourseLevel Level { get; set; }
    }
}