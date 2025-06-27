using Microsoft.EntityFrameworkCore;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Infrastructure.providers.TelegramProvider;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Services.Foundation
{
    public class NotificationService : INotificationService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ITelegramBotProvider telegramBotProvider;

        public NotificationService(
            IStorageBroker storageBroker,
            ITelegramBotProvider telegramBotProvider)
        {
            this.storageBroker = storageBroker ?? throw new NullArgumentException(nameof(storageBroker));
            this.telegramBotProvider = telegramBotProvider ?? throw new NullArgumentException(nameof(telegramBotProvider));
        }

        public async Task SendNotificationAsync(Notification notification)
        {
            if (notification == null || string.IsNullOrWhiteSpace(notification.Message))
                throw new ArgumentException("Notification or message cannot be null or empty.");

            try
            {
                if (notification.Channel == NotificationChannel.Telegram)
                {
                    if (string.IsNullOrWhiteSpace(notification.RecipientTelegramId))
                        throw new ArgumentException("RecipientTelegramId cannot be empty for Telegram channel.");
                    await telegramBotProvider.SendMessageAsync(
                        notification.RecipientTelegramId,
                        notification.Message);
                }
                else
                {
                    throw new NotImplementedException("Only Telegram channel is supported currently.");
                }

                notification.IsDelivered = true;
                notification.SentAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                notification.IsDelivered = false;
                notification.ErrorMessage = ex.Message;
            }

            await CreateNotificationAsync(notification);
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            if (notification == null || string.IsNullOrWhiteSpace(notification.Message))
                throw new ArgumentException("Notification cannot be null or have empty Message");

            notification.Id = Guid.NewGuid();
            notification.SentAt = notification.SentAt == default ? DateTime.UtcNow : notification.SentAt;
            return await storageBroker.InsertAsync(notification);
        }

        public async Task SendGroupNotificationAsync(string message, NotificationType type, NotificationPriority priority, int? categoryId = null, Guid? courseId = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.");

            var query = storageBroker.SelectAll<Student>()
                .Where(s => s.IsActive && !string.IsNullOrWhiteSpace(s.ParentTelegramId));

            if (courseId.HasValue)
            {
                query = query.Where(s => s.StudentCourses.Any(sc => sc.CourseId == courseId.Value));
            }
            else if (categoryId.HasValue)
            {
                query = query.Where(s => s.StudentCourses.Any(sc => sc.Course.CategoryId == categoryId.Value));
            }

            var students = await query.ToListAsync();

            foreach (var student in students)
            {
                var notification = new Notification
                {
                    StudentId = student.Id,
                    CourseId = courseId,
                    RecipientTelegramId = student.ParentTelegramId,
                    Message = message,
                    Type = type,
                    Channel = NotificationChannel.Telegram,
                    Priority = priority,
                    IsDelivered = false
                };
                await SendNotificationAsync(notification);
            }
        }
    }
}