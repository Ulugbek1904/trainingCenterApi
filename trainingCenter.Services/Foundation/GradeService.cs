using Microsoft.EntityFrameworkCore;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Services.Foundation
{
    public class GradeService : IGradeService
    {
        private readonly IStorageBroker storageBroker;
        private readonly INotificationService notificationService;

        public GradeService(IStorageBroker storageBroker, INotificationService notificationService)
        {
            this.storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<Grade> RegisterGradeAsync(Grade grade)
        {
            if (grade == null || grade.StudentId == Guid.Empty || grade.CourseId == Guid.Empty || grade.TeacherId == Guid.Empty)
                throw new ArgumentException("Grade cannot be null or have empty StudentId/CourseId/TeacherId");

            var student = await storageBroker.SelectByIdAsync<Student>(grade.StudentId);
            if (student == null)
                throw new ArgumentException($"Student with ID {grade.StudentId} not found");

            var course = await storageBroker.SelectByIdAsync<Course>(grade.CourseId);
            if (course == null)
                throw new ArgumentException($"Course with ID {grade.CourseId} not found");

            var teacher = await storageBroker.SelectByIdAsync<User>(grade.TeacherId);
            if (teacher == null || teacher.Role != Role.Teacher)
                throw new ArgumentException($"Teacher with ID {grade.TeacherId} not found or not a teacher");

            var createdGrade = await storageBroker.InsertAsync(grade);

            // Telegram xabari
            if (!string.IsNullOrWhiteSpace(student.ParentTelegramId))
            {
                var message = $"Hurmatli ota-ona, farzandingiz {student.FullName} {course.Name} kursida (Daraja: {course.Level}) {grade.Score} baho oldi. Izoh: {grade.Comment ?? "Yo'q"}";
                var notification = new Notification
                {
                    StudentId = student.Id,
                    CourseId = course.Id,
                    RecipientTelegramId = student.ParentTelegramId,
                    Message = message,
                    Type = NotificationType.GradeUpdate,
                    Channel = NotificationChannel.Telegram,
                    Priority = NotificationPriority.Normal,
                    IsDelivered = false
                };
                await notificationService.SendNotificationAsync(notification);
            }

            return createdGrade;
        }

        // Qolgan metodlar o'zgarmaydi
        public async Task<List<Grade>> RetrieveAllGradesAsync()
        {
            return await storageBroker.SelectAll<Grade>()
                .Include(g => g.Student)
                .Include(g => g.Course)
                .Include(g => g.Teacher)
                .ToListAsync();
        }

        public async Task<Grade> RetrieveGradeByIdAsync(Guid gradeId)
        {
            if (gradeId == Guid.Empty)
                throw new ArgumentException("Grade ID cannot be empty");

            var grade = await storageBroker.SelectByIdAsync<Grade>(gradeId);
            return grade ?? throw new NotFoundException($"Grade with ID {gradeId} not found");
        }

        public async Task<Grade> ModifyGradeAsync(Grade grade)
        {
            if (grade.Id == Guid.Empty)
                throw new ArgumentException("Grade ID cannot be empty");

            var existing = await storageBroker.SelectByIdAsync<Grade>(grade.Id);
            if (existing == null)
                throw new NotFoundException($"Grade with ID {grade.Id} not found");

            return await storageBroker.UpdateAsync(grade);
        }

        public async Task<Grade> RemoveGradeAsync(Guid gradeId)
        {
            if (gradeId == Guid.Empty)
                throw new ArgumentException("Grade ID cannot be empty");

            var grade = await storageBroker.SelectByIdAsync<Grade>(gradeId);
            if (grade == null)
                throw new NotFoundException($"Grade with ID {gradeId} not found");

            return await storageBroker.DeleteAsync(grade);
        }
    }
}