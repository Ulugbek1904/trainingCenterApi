using Microsoft.EntityFrameworkCore;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Services.Foundation
{
    public class CourseService : ICourseService
    {
        private readonly IStorageBroker storageBroker;

        public CourseService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker 
                ?? throw new NullArgumentException(nameof(storageBroker));
        }

        public async Task<Course> RegisterCourseAsync(Course course)
        {
            ValidateCourse(course);

            var teacher = await storageBroker.SelectByIdAsync<User>(course.TeacherId);
            if (teacher == null || teacher.Role != Role.Teacher)
                throw new NotFoundException($"Teacher with ID {course.TeacherId} not found or not a teacher");

            if (await storageBroker.SelectAll<Course>()
                .AnyAsync(c => c.Name == course.Name && c.CategoryId == course.CategoryId))
            {
                throw new ArgumentException("Course with the same name and category already exists");
            }

            return await storageBroker.InsertAsync(course);
        }

        public async Task<List<Course>> RetrieveAllCoursesAsync()
        {
            return await storageBroker.SelectAll<Course>()
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<Course> RetrieveCourseByIdAsync(Guid courseId)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty");

            var course = await storageBroker.SelectAll<Course>()
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == courseId);
            return course ?? throw new NotFoundException($"Course with ID {courseId} not found");
        }

        public async Task<Course> ModifyCourseAsync(Course course)
        {
            if (course.Id == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty");

            ValidateCourse(course);

            // TeacherId ni tekshirish
            var teacher = await storageBroker.SelectByIdAsync<User>(course.TeacherId);
            if (teacher == null || teacher.Role != Role.Teacher)
                throw new NotFoundException($"Teacher with ID {course.TeacherId} not found or not a teacher");

            var existing = await storageBroker.SelectByIdAsync<Course>(course.Id);
            if (existing == null)
                throw new NotFoundException($"Course with ID {course.Id} not found");

            return await storageBroker.UpdateAsync(course);
        }

        public async Task<Course> RemoveCourseAsync(Guid courseId)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty");

            var course = await storageBroker.SelectByIdAsync<Course>(courseId);
            if (course == null)
                throw new NotFoundException($"Course with ID {courseId} not found");

            return await storageBroker.DeleteAsync(course);
        }

        private static void ValidateCourse(Course course)
        {
            if (course == null || string.IsNullOrEmpty(course.Name))
                throw new ArgumentException("Course cannot be null or have empty Name");

            if (course.MaxStudents <= 0)
                throw new ArgumentException("MaxStudents must be greater than zero");

            if (course.Price < 0)
                throw new ArgumentException("Price cannot be negative");

            if (course.TeacherId == Guid.Empty)
                throw new ArgumentException("Teacher ID cannot be empty");
        }
    }
}