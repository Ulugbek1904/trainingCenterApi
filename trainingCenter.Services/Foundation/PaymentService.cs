using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Services.Foundation
{
    public class PaymentService : IPaymentService
    {
        private readonly IStorageBroker storageBroker;
        private readonly INotificationService notificationService;

        public PaymentService(IStorageBroker storageBroker, INotificationService notificationService)
        {
            this.storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<Payment> RegisterPaymentAsync(Payment payment)
        {
            if (payment == null || payment.StudentId == Guid.Empty || payment.CourseId == Guid.Empty)
                throw new ArgumentException("Payment cannot be null or have empty StudentId/CourseId");

            var student = await storageBroker.SelectByIdAsync<Student>(payment.StudentId);
            if (student == null)
                throw new ArgumentException($"Student with ID {payment.StudentId} not found");

            var course = await storageBroker.SelectByIdAsync<Course>(payment.CourseId);
            if (course == null)
                throw new ArgumentException($"Course with ID {payment.CourseId} not found");

            var createdPayment = await storageBroker.InsertAsync(payment);

            // Telegram xabari
            if (!string.IsNullOrWhiteSpace(student.ParentTelegramId))
            {
                var message = $"Hurmatli ota-ona, farzandingiz {student.FullName} uchun {course.Name} kursiga {payment.Amount} so'm to'lov qilindi. Sana: {payment.PaymentDate:yyyy-MM-dd}. Status: {payment.Status}";
                var notification = new Notification
                {
                    StudentId = student.Id,
                    CourseId = course.Id,
                    RecipientTelegramId = student.ParentTelegramId,
                    Message = message,
                    Type = NotificationType.PaymentUpdate,
                    Channel = NotificationChannel.Telegram,
                    Priority = NotificationPriority.Normal,
                    IsDelivered = false
                };
                await notificationService.SendNotificationAsync(notification);
            }

            return createdPayment;
        }

        public async Task NotifyPendingPaymentsAsync(Guid courseId)
        {
            var course = await storageBroker.SelectByIdAsync<Course>(courseId);
            if (course == null)
                throw new NotFoundException($"Course with ID {courseId} not found");

            var pendingPayments = await storageBroker.SelectAll<Payment>()
                .Where(p => p.CourseId == courseId && p.Status != "Paid" && p.DueDate <= DateTime.UtcNow.AddDays(7))
                .Include(p => p.Student)
                .ToListAsync();

            foreach (var payment in pendingPayments)
            {
                if (!string.IsNullOrWhiteSpace(payment.Student.ParentTelegramId))
                {
                    var message = $"Hurmatli ota-ona, farzandingiz {payment.Student.FullName} uchun {course.Name} kursiga {payment.Amount} so'm to'lov muddati yaqinlashmoqda (Sana: {payment.DueDate:yyyy-MM-dd}). Status: {payment.Status}";
                    var notification = new Notification
                    {
                        StudentId = payment.Student.Id,
                        CourseId = course.Id,
                        RecipientTelegramId = payment.Student.ParentTelegramId,
                        Message = message,
                        Type = NotificationType.PaymentUpdate,
                        Channel = NotificationChannel.Telegram,
                        Priority = NotificationPriority.High,
                        IsDelivered = false
                    };
                    await notificationService.SendNotificationAsync(notification);
                }
            }
        }

        // Qolgan metodlar o'zgarmaydi
        public async Task<List<Payment>> RetrieveAllPaymentsAsync()
        {
            return await storageBroker.SelectAll<Payment>()
                .Include(p => p.Student)
                .Include(p => p.Course)
                .ToListAsync();
        }

        public async Task<Payment> RetrievePaymentByIdAsync(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException("Payment ID cannot be empty");

            var payment = await storageBroker.SelectByIdAsync<Payment>(paymentId);
            return payment ?? throw new NotFoundException($"Payment with ID {paymentId} not found");
        }

        public async Task<Payment> ModifyPaymentAsync(Payment payment)
        {
            if (payment.Id == Guid.Empty)
                throw new ArgumentException("Payment ID cannot be empty");

            var existing = await storageBroker.SelectByIdAsync<Payment>(payment.Id);
            if (existing == null)
                throw new NotFoundException($"Payment with ID {payment.Id} not found");

            return await storageBroker.UpdateAsync(payment);
        }

        public async Task<Payment> RemovePaymentAsync(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException("Payment ID cannot be empty");

            var payment = await storageBroker.SelectByIdAsync<Payment>(paymentId);
            if (payment == null)
                throw new NotFoundException($"Payment with ID {paymentId} not found");

            return await storageBroker.DeleteAsync(payment);
        }
    }
}