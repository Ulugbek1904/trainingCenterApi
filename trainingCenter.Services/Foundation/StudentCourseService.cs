using Microsoft.EntityFrameworkCore;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Services.Foundation
{
    public class StudentCourseService : IStudentCourseService
    {
        private readonly IStorageBroker storageBroker;

        public StudentCourseService(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker ?? throw new NullArgumentException(nameof(storageBroker));
        }

        public async Task<StudentCourse> RegisterStudentCourseAsync(StudentCourse studentCourse)
        {
            if (studentCourse == null || studentCourse.StudentId == Guid.Empty || studentCourse.CourseId == Guid.Empty)
                throw new ArgumentException("StudentCourse cannot be null or have empty StudentId/CourseId");

            if (studentCourse.Discount < 0)
                throw new ArgumentException("Discount cannot be negative");

            if (await storageBroker.SelectAll<StudentCourse>()
                .AnyAsync(sc => sc.StudentId == studentCourse.StudentId && sc.CourseId == studentCourse.CourseId))
            {
                throw new ArgumentException("Student is already enrolled in this course");
            }

            var course = await storageBroker.SelectByIdAsync<Course>(studentCourse.CourseId);
            if (course == null)
                throw new NotFoundException($"Course with ID {studentCourse.CourseId} not found");

            var student = await storageBroker.SelectByIdAsync<Student>(studentCourse.StudentId);
            if (student == null)
                throw new NotFoundException($"Student with ID {studentCourse.StudentId} not found");

            return await storageBroker.InsertAsync(studentCourse);
        }

        public async Task<List<StudentCourse>> RetrieveAllStudentCoursesAsync()
        {
            return await storageBroker.SelectAll<StudentCourse>()
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .ToListAsync();
        }

        public async Task<StudentCourse> RetrieveStudentCourseByIdsAsync(Guid studentId, Guid courseId)
        {
            if (studentId == Guid.Empty || courseId == Guid.Empty)
                throw new ArgumentException("Student ID or Course ID cannot be empty");

            var studentCourse = await storageBroker.SelectAll<StudentCourse>()
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            return studentCourse ?? throw new NotFoundException($"StudentCourse with Student ID {studentId} and Course ID {courseId} not found");
        }

        public async Task<StudentCourse> ModifyStudentCourseAsync(StudentCourse studentCourse)
        {
            if (studentCourse == null || studentCourse.StudentId == Guid.Empty || studentCourse.CourseId == Guid.Empty)
                throw new ArgumentException("StudentCourse cannot be null or have empty StudentId/CourseId");

            if (studentCourse.Discount < 0)
                throw new ArgumentException("Discount cannot be negative");

            var existing = await storageBroker.SelectAll<StudentCourse>()
                .FirstOrDefaultAsync(sc => sc.StudentId == studentCourse.StudentId && sc.CourseId == studentCourse.CourseId);

            if (existing == null)
                throw new NotFoundException($"StudentCourse with Student ID {studentCourse.StudentId} and Course ID {studentCourse.CourseId} not found");

            return await storageBroker.UpdateAsync(studentCourse);
        }

        public async Task<StudentCourse> RemoveStudentCourseAsync(Guid studentId, Guid courseId)
        {
            if (studentId == Guid.Empty || courseId == Guid.Empty)
                throw new ArgumentException("Student ID or Course ID cannot be empty");

            var studentCourse = await storageBroker.SelectAll<StudentCourse>()
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (studentCourse == null)
                throw new NotFoundException($"StudentCourse with Student ID {studentId} and Course ID {courseId} not found");

            return await storageBroker.DeleteAsync(studentCourse);
        }
    }
}