using trainingCenter.Domain.Models;

namespace trainingCenter.Services.Foundation.Interfaces;

public interface IStudentCourseService
{
    Task<StudentCourse> RegisterStudentCourseAsync(StudentCourse studentCourse);
    Task<List<StudentCourse>> RetrieveAllStudentCoursesAsync();
    Task<StudentCourse> RetrieveStudentCourseByIdsAsync(Guid studentId, Guid courseId);
    Task<StudentCourse> ModifyStudentCourseAsync(StudentCourse studentCourse);
    Task<StudentCourse> RemoveStudentCourseAsync(Guid studentId, Guid courseId);
}