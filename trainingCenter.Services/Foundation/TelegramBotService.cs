using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Infrastructure.providers.TelegramProvider;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenter.Services.Foundation
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ITelegramBotProvider telegramBotProvider;
        private readonly IStorageBroker storageBroker;

        public TelegramBotService(ITelegramBotProvider telegramBotProvider, IStorageBroker storageBroker)
        {
            this.telegramBotProvider = telegramBotProvider ?? throw new ArgumentNullException(nameof(telegramBotProvider));
            this.storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        }

        public async Task RegisterParentAsync(string parentTelegramId, string parentPhoneNumber)
        {
            string phoneNumber = "+" + parentPhoneNumber?.Trim();
            if (string.IsNullOrWhiteSpace(parentTelegramId))
                throw new ValidationException("Parent Telegram ID cannot be empty.");
            if (string.IsNullOrWhiteSpace(parentPhoneNumber))
                throw new ValidationException("Parent phone number cannot be empty.");

            var students = storageBroker.SelectAll<Student>()
                .Where(s => s.ParentPhoneNumber == phoneNumber)
                .ToList();

            if (!students.Any())
                throw new NotFoundException($"No students found with parent phone number {parentPhoneNumber}.");

            foreach (var student in students)
            {
                student.ParentTelegramId = parentTelegramId;
                await storageBroker.UpdateAsync(student);
            }

            var studentNames = string.Join(", ", students.Select(s => s.FullName));
            await telegramBotProvider.SendMessageAsync(parentTelegramId,
                $"Siz {studentNames} ota-onasi sifatida ro‘yxatdan o‘tdingiz.");
        }

        public async Task SendMessageToParentAsync(Guid studentId, string message)
        {
            if (studentId == Guid.Empty)
                throw new ValidationException("Student ID cannot be empty.");

            if (string.IsNullOrWhiteSpace(message))
                throw new ValidationException("Message cannot be empty.");

            var student = await storageBroker.SelectByIdAsync<Student>(studentId);

            if (student == null)
                throw new NotFoundException($"Student with ID {studentId} not found.");

            if (string.IsNullOrWhiteSpace(student.ParentTelegramId))
                throw new ValidationException($"Parent Telegram ID not set for student {student.FullName}.");

            var formattedMessage = $"Hurmatli ota-ona, {student.FullName}: {message}";

            await telegramBotProvider.SendMessageAsync(student.ParentTelegramId, formattedMessage);
        }

        public async Task SendMessageToAllParentsAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ValidationException("Message cannot be empty.");

            var students = storageBroker.SelectAll<Student>()
                .Where(s => !string.IsNullOrWhiteSpace(s.ParentTelegramId))
                .ToList();

            if (!students.Any())
                throw new NotFoundException("No students with registered parents found.");

            var uniqueParentIds = students
                .Select(s => s.ParentTelegramId)
                .Distinct()
                .ToList();

            foreach (var parentTelegramId in uniqueParentIds)
            {
                var parentStudents = students
                    .Where(s => s.ParentTelegramId == parentTelegramId)
                    .Select(s => s.FullName)
                    .ToList();

                var formattedMessage = $"Hurmatli ota-ona, farzandlaringiz ({string.Join(", ", parentStudents)}): {message}";
                await telegramBotProvider.SendMessageAsync(parentTelegramId, formattedMessage);
            }
        }
    }
}