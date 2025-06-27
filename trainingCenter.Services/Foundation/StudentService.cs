using Microsoft.EntityFrameworkCore;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Services.Foundation;

public class StudentService : IStudentService
{
    private readonly IStorageBroker storageBroker;

    public StudentService(IStorageBroker storageBroker)
    {
        this.storageBroker = storageBroker;
    }

    public async ValueTask<Student> RegisterStudentAsync(Student student)
    {
        ValidateStudent(student);

        if ( await this.storageBroker.SelectAll<Student>()
            .AnyAsync(s => s.FullName == student.FullName 
            && s.PhoneNumber == student.PhoneNumber))
        {
            throw new ArgumentException("Student with the same phone number already exists");
        }

        return await this.storageBroker.InsertAsync(student);
    }

    public ValueTask<IQueryable<Student>> RetrieveAllStudents()
    {
        var students = this.storageBroker.SelectAll<Student>();

        return new ValueTask<IQueryable<Student>>(students);
    }

    public async ValueTask<Student> RetrieveStudentByIdAsync(Guid studentId)
    {
        var student = await this.storageBroker
            .SelectByIdAsync<Student>(studentId);

        if (student is null)
            throw new NotFoundException($"Student with ID {studentId} not found");

        return student;
    }

    public async ValueTask<Student> ModifyStudentAsync(Student student)
    {
        ValidateStudent(student);

        return await this.storageBroker.UpdateAsync(student);
    }

    public ValueTask<Student> RemoveStudentAsync(Guid studentId)
    {
        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("Student ID cannot be empty");
        }

        return this.storageBroker.DeleteAsync(new Student { Id = studentId });
    }

    private static void ValidateStudent(Student student)
    {
        if (student == null || string.IsNullOrEmpty(student.FullName))
            throw new ArgumentException("Student cannot be null or have empty FullName");

        if (student.BirthDate > DateTime.UtcNow)
            throw new ArgumentException("Birth date cannot be in the future");

        if (string.IsNullOrEmpty(student.PhoneNumber) && string.IsNullOrEmpty(student.ParentPhoneNumber))
            throw new ArgumentException("At least Parent phone number must be provided");
    }
}
