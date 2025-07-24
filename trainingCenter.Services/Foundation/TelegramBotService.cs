using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;
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

        public TelegramBotService(
            ITelegramBotProvider telegramBotProvider,
            IStorageBroker storageBroker)
        {
            this.telegramBotProvider = telegramBotProvider;
            this.storageBroker = storageBroker;
        }

        public async Task RegisterParentAsync(string telegramId, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(telegramId))
                throw new ValidationException("Telegram ID bo‘sh bo‘lishi mumkin emas");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ValidationException("Telefon raqam bo‘sh bo‘lishi mumkin emas");

            if (!phoneNumber.StartsWith("+"))
                phoneNumber = "+" + phoneNumber;

            var students = await storageBroker.SelectAll<Student>()
                .Where(s => s.ParentPhoneNumber == phoneNumber)
                .ToListAsync();

            if (!students.Any())
                throw new NotFoundException("Bu telefon raqamga mos keluvchi farzand topilmadi.");

            foreach (var student in students)
            {
                student.ParentTelegramId = telegramId;
                await storageBroker.UpdateAsync(student);
            }

            var tenantId = students.First().TenantId;
            var studentNames = string.Join(", ", students.Select(s => s.FullName));

            await telegramBotProvider.SendMessageAsync(
                tenantId,
                telegramId,
                $"Siz quyidagi farzandlar uchun ro‘yxatdan o‘tdingiz: {studentNames}.");
        }

        public async Task SendMenuAsync(Guid tenantId, string telegramId)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📊 Hisobot", $"report_menu")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("❓ Yordam", $"help")
                }
            });

            await telegramBotProvider.SendMessageAsync(
                tenantId,
                telegramId,
                "Quyidagi tugmalardan foydalaning:",
                replyMarkup: keyboard);
        }

        public async Task SendReportMenuAsync(Guid tenantId, string telegramId)
        {
            var students = await storageBroker.SelectAll<Student>()
                .Where(s => s.ParentTelegramId == telegramId && s.TenantId == tenantId)
                .ToListAsync();

            if (!students.Any())
            {
                await telegramBotProvider.SendMessageAsync(tenantId, telegramId, "Sizning farzandlaringiz topilmadi.");
                return;
            }

            var keyboard = new InlineKeyboardMarkup(
                students.Select(s => new[]
                {
                    InlineKeyboardButton.WithCallbackData(s.FullName, $"select_student_{s.Id}")
                })
            );

            await telegramBotProvider.SendMessageAsync(
                tenantId,
                telegramId,
                "Farzandingizni tanlang:",
                replyMarkup: keyboard);
        }

        public async Task SendReportOptionsAsync(Guid tenantId, string telegramId, Guid studentId)
        {
            var student = await storageBroker.SelectByIdAsync<Student>(studentId);

            if (student == null || student.ParentTelegramId != telegramId || student.TenantId != tenantId)
            {
                await telegramBotProvider.SendMessageAsync(tenantId, telegramId, "Farzand topilmadi yoki ruxsat yo‘q.");
                return;
            }

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] {
                    InlineKeyboardButton.WithCallbackData("Davomat", $"report_attendance_{studentId}")
                },
                new[] {
                    InlineKeyboardButton.WithCallbackData("Baholar", $"report_grades_{studentId}")
                }
            });

            await telegramBotProvider.SendMessageAsync(
                tenantId,
                telegramId,
                $"{student.FullName} uchun hisobot turini tanlang:",
                replyMarkup: keyboard);
        }

        public async Task SendHelpAsync(Guid tenantId, string telegramId)
        {
            await telegramBotProvider.SendMessageAsync(
                tenantId,
                telegramId,
                "Yordam uchun:\n - /start: Bosh menyu\n - /register: Ro‘yxatdan o‘tish\n - /report: Hisobotlar");
        }

        public async Task HandleRegisterPhoneNumberAsync(Guid tenantId, string telegramId, string phoneNumber)
        {
            try
            {
                await RegisterParentAsync(telegramId, phoneNumber);
                await telegramBotProvider.SendMessageAsync(
                    tenantId,
                    telegramId,
                    "Ro‘yxatdan o‘tish muvaffaqiyatli yakunlandi!");
            }
            catch (NotFoundException ex)
            {
                await telegramBotProvider.SendMessageAsync(
                    tenantId,
                    telegramId,
                    "Bu telefon raqamga mos keluvchi farzand topilmadi. Iltimos, to‘g‘ri raqam kiriting.");
            }
            catch (ValidationException ex)
            {
                await telegramBotProvider.SendMessageAsync(
                    tenantId,
                    telegramId,
                    ex.Message);
            }
        }

    }
}
