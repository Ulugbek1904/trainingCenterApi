using trainingCenter.Domain.Models;

namespace trainingCenter.Services.Foundation.Interfaces;

public interface ICourseService
{
    Task<Course> RegisterCourseAsync(Course course);
    Task<List<Course>> RetrieveAllCoursesAsync();
    Task<Course> RetrieveCourseByIdAsync(Guid courseId);
    Task<Course> ModifyCourseAsync(Course course);
    Task<Course> RemoveCourseAsync(Guid courseId);
}
