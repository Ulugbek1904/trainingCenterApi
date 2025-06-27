using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Services.Foundation
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IStorageBroker storageBroker;
        private readonly INotificationService notificationService;

        public AttendanceService(IStorageBroker storageBroker, INotificationService notificationService)
        {
            this.storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<Attendance> RegisterAttendanceAsync(Attendance attendance)
        {
            if (attendance == null || attendance.StudentId == Guid.Empty || attendance.CourseId == Guid.Empty)
                throw new ArgumentException("Attendance cannot be null or have empty StudentId/CourseId");

            var student = await storageBroker.SelectByIdAsync<Student>(attendance.StudentId);
            if (student == null)
                throw new ArgumentException($"Student with ID {attendance.StudentId} not found");

            var course = await storageBroker.SelectByIdAsync<Course>(attendance.CourseId);
            if (course == null)
                throw new ArgumentException($"Course with ID {attendance.CourseId} not found");

            var createdAttendance = await storageBroker.InsertAsync(attendance);

            // Telegram xabari
            if (!string.IsNullOrWhiteSpace(student.ParentTelegramId))
            {
                var statusText = attendance.Status switch
                {
                    Status.keldi => "darsda qatnashdi",
                    Status.kelmadi => "darsda qatnashmadi",
                    Status.kechikdi => "darsga kechikdi",
                    Status.sababli => "sababli kelmadi",
                    _ => "noma'lum holat"
                };
                var message = $"Hurmatli ota-ona, farzandingiz " +
                    $"{student.FullName} {course.Name} kursida (Daraja: {course.Level})" +
                    $" {statusText}. Izoh: {attendance.Notes ?? "Yo'q"}";
                var notification = new Notification
                {
                    StudentId = student.Id,
                    CourseId = course.Id,
                    RecipientTelegramId = student.ParentTelegramId,
                    Message = message,
                    Type = NotificationType.AttendanceUpdate,
                    Channel = NotificationChannel.Telegram,
                    Priority = NotificationPriority.Normal,
                    IsDelivered = false
                };
                await notificationService.SendNotificationAsync(notification);
            }

            return createdAttendance;
        }

        // Qolgan metodlar o'zgarmaydi
        public async Task<List<Attendance>> RetrieveAllAttendancesAsync()
        {
            return await storageBroker.SelectAll<Attendance>()
                .Include(a => a.Student)
                .Include(a => a.Course)
                .ToListAsync();
        }

        public async Task<Attendance> RetrieveAttendanceByIdAsync(Guid attendanceId)
        {
            if (attendanceId == Guid.Empty)
                throw new ArgumentException("Attendance ID cannot be empty");

            var attendance = await storageBroker.SelectByIdAsync<Attendance>(attendanceId);
            return attendance ?? throw new NotFoundException($"Attendance with ID {attendanceId} not found");
        }

        public async Task<Attendance> ModifyAttendanceAsync(Attendance attendance)
        {
            if (attendance.Id == Guid.Empty)
                throw new ArgumentException("Attendance ID cannot be empty");

            var existing = await storageBroker.SelectByIdAsync<Attendance>(attendance.Id);
            if (existing == null)
                throw new NotFoundException($"Attendance with ID {attendance.Id} not found");

            return await storageBroker.UpdateAsync(attendance);
        }

        public async Task<Attendance> RemoveAttendanceAsync(Guid attendanceId)
        {
            if (attendanceId == Guid.Empty)
                throw new ArgumentException("Attendance ID cannot be empty");

            var attendance = await storageBroker.SelectByIdAsync<Attendance>(attendanceId);
            if (attendance == null)
                throw new NotFoundException($"Attendance with ID {attendanceId} not found");

            return await storageBroker.DeleteAsync(attendance);
        }
    }
}