using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Dtos;
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
        private readonly IReportService reportService;

        public TelegramBotService(
            ITelegramBotProvider telegramBotProvider,
            IStorageBroker storageBroker,
            IReportService reportService)
        {
            this.telegramBotProvider = telegramBotProvider
                ?? throw new NullArgumentException(nameof(telegramBotProvider));
            this.storageBroker = storageBroker
                ?? throw new NullArgumentException(nameof(storageBroker));
            this.reportService = reportService
                ?? throw new NullArgumentException(nameof(reportService));
        }

        public async Task RegisterParentAsync(string parentTelegramId, string parentPhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(parentTelegramId))
                throw new ValidationException("Parent Telegram ID cannot be empty.");
            if (string.IsNullOrWhiteSpace(parentPhoneNumber))
                throw new ValidationException("Parent phone number cannot be empty.");

            if (!parentPhoneNumber.StartsWith("+"))
            {
                parentPhoneNumber = "+" + parentPhoneNumber;
            }
            var students = storageBroker.SelectAll<Student>()
                .Where(s => s.ParentPhoneNumber == parentPhoneNumber)
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

        public async Task SendReportMenuAsync(string parentTelegramId)
        {
            var students = await storageBroker.SelectAll<Student>()
                .Where(s => s.ParentTelegramId == parentTelegramId)
                .ToListAsync();

            if (!students.Any())
            {
                await telegramBotProvider.SendMessageAsync(parentTelegramId, "Sizning farzandlaringiz ro‘yxatdan o‘tmagan.");
                return;
            }

            var keyboard = new InlineKeyboardMarkup(
                students.Select(s => new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        s.FullName,
                        $"select_student_{s.Id}")
                }).ToArray()
            );

            await telegramBotProvider.SendMessageAsync(
                parentTelegramId,
                "Farzandingizni tanlang:",
                replyMarkup: keyboard);
        }

        public async Task SendReportOptionsAsync(string parentTelegramId, Guid studentId)
        {
            var student = await storageBroker.SelectByIdAsync<Student>(studentId);
            if (student == null || student.ParentTelegramId != parentTelegramId)
            {
                await telegramBotProvider.SendMessageAsync(parentTelegramId, "Farzand topilmadi yoki ruxsat yo‘q.");
                return;
            }

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Davomat (1 hafta)", $"report_attendance_{studentId}_week"),
                    InlineKeyboardButton.WithCallbackData("Davomat (1 oy)", $"report_attendance_{studentId}_month")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Baholar (1 hafta)", $"report_grades_{studentId}_week"),
                    InlineKeyboardButton.WithCallbackData("Baholar (1 oy)", $"report_grades_{studentId}_month")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("To‘lovlar", $"report_payments_{studentId}")
                }
            });

            await telegramBotProvider.SendMessageAsync(
                parentTelegramId,
                $"{student.FullName} uchun hisobot turini tanlang:",
                replyMarkup: keyboard);
        }

        public async Task SendReportAsync(string parentTelegramId, string callbackData)
        {
            var parts = callbackData.Split('_');
            if (parts.Length < 3)
            {
                await telegramBotProvider.SendMessageAsync(parentTelegramId, "Noto‘g‘ri so‘rov.");
                return;
            }

            var type = parts[1];
            var studentId = Guid.Parse(parts[2]);
            var period = parts.Length > 3 ? parts[3] : null;

            var student = await storageBroker.SelectByIdAsync<Student>(studentId);
            if (student == null || student.ParentTelegramId != parentTelegramId)
            {
                await telegramBotProvider.SendMessageAsync(parentTelegramId, "Farzand topilmadi yoki ruxsat yo‘q.");
                return;
            }

            DateTime? startDate = period switch
            {
                "week" => DateTime.UtcNow.AddDays(-7),
                "month" => DateTime.UtcNow.AddMonths(-1),
                _ => null
            };
            DateTime? endDate = DateTime.UtcNow;

            var report = await reportService.GetStudentReportAsync(studentId, startDate, endDate);

            string message = type switch
            {
                "attendance" => FormatAttendanceReport(report),
                "grades" => FormatGradeReport(report),
                "payments" => FormatPaymentReport(report),
                _ => "Noto‘g‘ri hisobot turi."
            };

            await telegramBotProvider.SendMessageAsync(parentTelegramId, message);
        }

        private string FormatAttendanceReport(StudentReportDto report)
        {
            if (!report.AttendanceHistory.Any())
                return $"Hurmatli ota-ona, {report.FullName} uchun davomat ma'lumotlari yo‘q.";

            var message = $"Hurmatli ota-ona, {report.FullName} uchun davomat:\n\n";
            foreach (var att in report.AttendanceHistory)
            {
                message += $"Sana: {att.Date:yyyy-MM-dd}\n";
                message += $"Kurs: {att.CourseName}\n";
                message += $"Holati: {att.Status}\n";
                message += $"Izoh: {att.Comment}\n\n";
            }
            message += $"Jami darslar: {report.TotalAttendanceCount}\n";
            message += $"Jami soat: {report.TotalAttendanceHours}";
            return message;
        }

        private string FormatGradeReport(StudentReportDto report)
        {
            if (!report.GradeHistory.Any())
                return $"Hurmatli ota-ona, {report.FullName} uchun baho ma'lumotlari yo‘q.";

            var message = $"Hurmatli ota-ona, {report.FullName} uchun baholar:\n\n";
            foreach (var grade in report.GradeHistory)
            {
                message += $"Sana: {grade.Date:yyyy-MM-dd}\n";
                message += $"Kurs: {grade.CourseName}\n";
                message += $"Baho: {grade.Score}\n";
                message += $"Izoh: {grade.Comment}\n\n";
            }
            return message;
        }

        private string FormatPaymentReport(StudentReportDto report)
        {
            if (!report.PaymentHistory.Any())
                return $"Hurmatli ota-ona, {report.FullName} uchun to‘lov ma'lumotlari yo‘q.";

            var message = $"Hurmatli ota-ona, {report.FullName} uchun to‘lovlar:\n\n";
            foreach (var payment in report.PaymentHistory)
            {
                message += $"To‘lov sanasi: {payment.PaymentDate:yyyy-MM-dd}\n";
                message += $"Muddati: {(payment.DueDate.HasValue ? payment.DueDate.Value.ToString("yyyy-MM-dd") : "Yo‘q")}\n";
                message += $"Summa: {payment.Amount} so‘m\n";
                message += $"Status: {payment.Status}\n";
                message += $"Kurs: {payment.CourseName}\n";
                message += $"To‘lov rejasi: {payment.InstallmentPlan}\n\n";
            }
            return message;
        }
    }
}