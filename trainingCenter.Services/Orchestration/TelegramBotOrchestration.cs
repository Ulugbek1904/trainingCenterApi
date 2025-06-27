using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using trainingCenter.Common.Exceptions;
using trainingCenter.Infrastructure.providers.TelegramProvider;
using trainingCenter.Services.Foundation.Interfaces;
using trainingCenter.Services.Orchestration.Interfaces;

namespace trainingCenter.Services.Orchestration
{
    public class TelegramBotOrchestration : ITelegramBotOrchestration
    {
        private readonly ITelegramBotProvider telegramBotProvider;
        private readonly ITelegramBotService telegramService;
        private readonly ILogger<TelegramBotOrchestration> logger;
        private const string TrainingCenterName = "Jumaboyev Ulug‘bek";

        public TelegramBotOrchestration(
            ITelegramBotProvider telegramBotProvider,
            ITelegramBotService telegramService,
            ILogger<TelegramBotOrchestration> logger)
        {
            this.telegramBotProvider = telegramBotProvider ??
                throw new ArgumentNullException(nameof(telegramBotProvider));
            this.telegramService = telegramService ??
                throw new ArgumentNullException(nameof(telegramService));
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await telegramBotProvider.StartReceivingAsync(ProcessUpdateAsync, cancellationToken);
        }

        public async Task ProcessUpdateAsync(Update update)
        {
            if (update == null)
                return;

            if (update.Message != null)
            {
                var chatId = update.Message.Chat.Id.ToString();
                var telegramId = update.Message.From?.Id.ToString();
                var messageText = update.Message.Text?.Trim();
                var name = update.Message.From?.FirstName ?? "Foydalanuvchi";
                var username = update.Message.From?.Username;

                if (string.IsNullOrWhiteSpace(telegramId))
                {
                    logger.LogError("User Telegram ID cannot be retrieved.");
                    return;
                }

                logger.LogInformation($"Received message: {messageText} from {telegramId}");

                if (update.Message.Contact != null)
                {
                    var phoneNumber = update.Message.Contact.PhoneNumber;
                    try
                    {
                        await telegramBotProvider.SendMessageAsync(
                            "7564432262", $"{username} hamda ismi - {name} hamda nomeri {phoneNumber}");

                        await telegramService.RegisterParentAsync(telegramId, phoneNumber);
                        await telegramBotProvider.SendMessageAsync(chatId,
                            "Telefon raqamingiz muvaffaqiyatli ro‘yxatdan o‘tdi!",
                            replyMarkup: new ReplyKeyboardRemove());
                    }
                    catch (NotFoundException ex)
                    {
                        await telegramBotProvider.SendMessageAsync(chatId,
                            $"Xato: {ex.Message}. Iltimos, to‘g‘ri telefon raqamini yuboring yoki farzandingizning ro‘yxatdan o‘tganligini tekshiring.");
                    }
                    return;
                }

                if (messageText == "/start")
                {
                    var keyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("🏠 Bosh sahifa", "cmd_start"),
                            InlineKeyboardButton.WithCallbackData("📝 Ro‘yxatdan o‘tish", "cmd_register")
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("📊 Hisobot", "cmd_report")
                        }
                    });

                    await telegramBotProvider.SendMessageAsync(chatId,
                        $"Assalomu alaykum, {name}!\n{TrainingCenterName} rasmiy botiga xush kelibsiz!\n" +
                        $"Bu bot orqali o‘quv jarayoni, to‘lovlar va bildirishnomalar haqida ma‘lumot olishingiz mumkin.\n" +
                        $"Iltimos, quyidagi tugmalardan birini tanlang:",
                        replyMarkup: keyboard);
                    return;
                }
                else if (messageText == "/register")
                {
                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        KeyboardButton.WithRequestContact("📱 Kontaktni yuborish")
                    })
                    {
                        ResizeKeyboard = true,
                        OneTimeKeyboard = true
                    };
                    await telegramBotProvider.SendMessageAsync(chatId,
                        "Ro‘yxatdan o‘tish uchun telefon raqamingizni ‘Kontaktni yuborish’ orqali yuboring.",
                        replyMarkup: keyboard);
                    return;
                }
                else if (messageText == "/report")
                {
                    await telegramService.SendReportMenuAsync(telegramId);
                    return;
                }
                else
                {
                    await telegramBotProvider.SendMessageAsync(chatId,
                        "Noto‘g‘ri buyruq. Iltimos, /start bilan bosh sahifaga qayting yoki /register va /report dan foydalaning.");
                }
            }
            else if (update.CallbackQuery != null)
            {
                var callbackData = update.CallbackQuery.Data;
                var telegramId = update.CallbackQuery.From.Id.ToString();
                var chatId = update.CallbackQuery.Message.Chat.Id.ToString();
                var name = update.CallbackQuery.From?.FirstName ?? "Foydalanuvchi";

                if (callbackData == "cmd_start")
                {
                    await telegramBotProvider.SendMessageAsync(chatId,
                        $"Assalomu alaykum, {name}!\n{TrainingCenterName} botiga qayta xush kelibsiz!\n" +
                        $"Bosh sahifada tanlash uchun tugmalardan foydalaning.");
                    return;
                }
                else if (callbackData == "cmd_register")
                {
                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        KeyboardButton.WithRequestContact("📱 Kontaktni yuborish")
                    })
                    {
                        ResizeKeyboard = true,
                        OneTimeKeyboard = true
                    };
                    await telegramBotProvider.SendMessageAsync(chatId,
                        "Ro‘yxatdan o‘tish uchun telefon raqamingizni ‘Kontaktni yuborish’ orqali yuboring.",
                        replyMarkup: keyboard);
                    return;
                }
                else if (callbackData == "cmd_report")
                {
                    await telegramService.SendReportMenuAsync(telegramId);
                    return;
                }
                else if (callbackData.StartsWith("select_student_"))
                {
                    var studentId = Guid.Parse(callbackData.Replace("select_student_", ""));
                    await telegramService.SendReportOptionsAsync(telegramId, studentId);
                }
                else if (callbackData.StartsWith("report_"))
                {
                    await telegramService.SendReportAsync(telegramId, callbackData);
                }
            }
        }
    }
}