using trainingCenter.Domain.Models;

namespace trainingCenter.Services.Foundation.Interfaces;

public interface IStudentService
{
    ValueTask<Student> RegisterStudentAsync(Student student);
    ValueTask<Student> RetrieveStudentByIdAsync(Guid studentId);
    ValueTask<Student> ModifyStudentAsync(Student student);
    ValueTask<Student> RemoveStudentAsync(Guid studentId);
    ValueTask<IQueryable<Student>> RetrieveAllStudents();
}
